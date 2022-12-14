using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class AbstractCharacter : AbstractSprite
{
    private float _healthPoints;
    public float HealthPoints
    {
        get
        {
            return _healthPoints;
        }

        set
        {
            _healthPoints = value;
            
            string className = GetType().Name;
            hpText.SetText($"{className} HP: {_healthPoints}");
        }
    }
    protected float MaxHealthPoints;
    public float JumpForce = 20f;
    
    protected bool CanAttack = true;
    protected float AttackDelayInSeconds;
    
    private LayerMask m_MakeJumpPossibleLayer;
    

    protected BoxCollider2D BoxCollider2D;
    protected Rigidbody2D m_Rb2D;
    private readonly float m_BottomHitDistance = 0.1f;

    public TMP_Text hpText; 
    
    public new void Awake()
    {
        base.Awake();
        string[] layers = { "Ground", "Platforms" };
        m_MakeJumpPossibleLayer = LayerMask.GetMask(layers);
        
        BoxCollider2D = GetComponent<BoxCollider2D>();
        m_Rb2D = GetComponent<Rigidbody2D>();
        m_Rb2D.gravityScale = 6f;
        
        ResetLife();
    }
    
    public new void Start()
    {
        base.Start();
        HandleAttackDone();
    }

    public new void Update()
    {
        base.Update();
    }

    public void LooseHp(float hpCountToLoose)
    {
        if (IsDead())
        {
            return;
        }
        
        HealthPoints -= hpCountToLoose;
        
        if (IsDead())
        {
            HandleDeath();
        }
        else
        {
            BlinkSprite();
            HandleHpLost();
        }
    }

    public void Jump()
    {
        if (IsHittingDown())
        {
            m_Rb2D.velocity = Vector2.up * JumpForce;
        }
    }

    public void ResetLife()
    {
        HealthPoints = MaxHealthPoints;
    }
    
    public bool IsHittingDown()
    {
        var bc2DBounds = BoxCollider2D.bounds;
        var halfSpriteHeight = bc2DBounds.size.y / 2;
        var bottomSpritePosition = bc2DBounds.center + (Vector3.down * halfSpriteHeight);
        
        var hitDown = Physics2D.Raycast(
            bottomSpritePosition, 
            Vector2.down, 
            m_BottomHitDistance,
            m_MakeJumpPossibleLayer
        );
        
        Debug.DrawRay(bottomSpritePosition, Vector3.down * m_BottomHitDistance);


        return hitDown.collider;
    }

    public bool IsDead()
    {
        return HealthPoints <= 0;
    }

    protected void BlinkSprite()
    {
        StartCoroutine(AnimateBlinkSprite());
    }
    
    IEnumerator AnimateBlinkSprite()
    {
        float maxA = 1f;
        float minA = 0.2f;
        float stepA = 0.1f;
        float intervalTime = 0.01f;

        for (int i = 0; i < 4; i++)
        {
            for(var j=maxA; j>minA; j-=stepA)
            {
                SpriteRenderer.color = new Color(1, 1, 1, j);
                yield return new WaitForSeconds(intervalTime);
            }
            for(var j=minA; j<maxA; j+=stepA)
            {
                SpriteRenderer.color = new Color(1, 1, 1, j);
                yield return new WaitForSeconds(intervalTime);
            }
        }
        
       
        SpriteRenderer.color = new Color(1, 1, 1, 1);
    }
    
    protected virtual void HandleHpLost()
    {
        string className = GetType().Name;
        Debug.LogWarning($"Character {className} lost HP (now at {HealthPoints}), not Handled");
    }
    
    protected virtual void HandleDeath()
    {
        string className = GetType().Name;
        Debug.LogWarning($"Character {className} dead, not handled");
    }
    
    protected virtual void HandleAttackDone()
    {
        CanAttack = false;
        StartCoroutine(ApplyAttackDelay());
    }
    
    protected virtual IEnumerator ApplyAttackDelay()
    {
        yield return new WaitForSeconds(AttackDelayInSeconds);
        CanAttack = true;
    }
}
