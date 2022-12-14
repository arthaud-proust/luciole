using System.Collections;
using System.Collections.Generic;
using Prefabs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Characters
{
    public class Hero : AbstractCharacter
    {
        protected float ShortRangeAttackPoints;
        
        public GameObject flowerPrefab;
        
        public bool canShortRangeAttack = true;
        public bool canLongRangeAttack = true;

        protected Animator Animator;
        
        protected Hero()
        {
            MovingSpeed = 5f;
            JumpForce = 25f;
            ShortRangeAttackPoints = 20f;
            AttackDelayInSeconds = 0.3f;
            MaxHealthPoints = 6f;
        }

        new void Awake()
        {
            base.Awake();
            Animator = GetComponent<Animator>();
            // boss = GameObject.Find("Boss1").GetComponent<AbstractBoss>();
        }

        new void Start()
        {
            base.Start();
        }

        new void Update()
        {
            base.Update();

            CheckMove();
            CheckJump();
            CheckAttack();
        }
        
        void CheckMove()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Move(Vector3.right);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Move(Vector3.left);
            }
        }

        void CheckJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        
        void CheckAttack()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                ShortRangeAttack();
            }
            
            if (Input.GetKeyDown(KeyCode.V))
            {
                LongRangeAttack();
            }
        }



        protected void ShortRangeAttack()
        {
            if (!CanAttack || !canShortRangeAttack)
            {
                return;
            }

            var hitGameObjectList = Physics2D.CircleCastAll(
                transform.position,
                3f,
                Vector2.zero
            );

            foreach (var hitGameObject in hitGameObjectList)
            {

                var characterHit = hitGameObject.collider.gameObject.GetComponent<AbstractCharacter>();

                if (characterHit && characterHit.GetType().IsSubclassOf(typeof(AbstractBoss)))
                {
                    characterHit.LooseHp(ShortRangeAttackPoints);
                }
            }

            HandleAttackDone();
        }

        protected void LongRangeAttack()
        {
            if (!CanAttack || !canLongRangeAttack)
            {
                return;
            }
            
            var halfSpriteHeight = SpriteRenderer.bounds.size.y/2;
            var topSpritePosition = transform.position + (Vector3.up * halfSpriteHeight);

            var projectile = GameObject.Instantiate(
                flowerPrefab, 
                transform.position, 
                flowerPrefab.transform.rotation
            );

            var projectileClass = projectile.GetComponent<Flower>();
            
            projectileClass.Rb2D.velocity = new Vector2(
                (SpriteRenderer.flipX ? -1 : 1) * 10,
                2
            );

            HandleAttackDone();
        }
        
        protected override void HandleDeath()
        {
            SceneManager.LoadScene("GameLost");
        }

        protected override void HandleAttackDone()
        {
            CanAttack = false;
            Animator.SetBool("IsAttacking", true);
            StartCoroutine(ApplyAttackDelay());
        }
     
        protected override IEnumerator ApplyAttackDelay()
        {
            yield return new WaitForSeconds(AttackDelayInSeconds);
            CanAttack = true;
            Animator.SetBool("IsAttacking", false);
        }
    }
}