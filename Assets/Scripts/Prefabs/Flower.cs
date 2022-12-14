using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Prefabs
{
    public class Flower: AffectingSprite
    {
        private const float FlowerDamage = 5f;
        
        public override bool IsCharacterATarget(AbstractCharacter character)
        {
            return character.GetType().IsSubclassOf(typeof(AbstractBoss));
        }

        public override void AffectTargetCharacter(AbstractCharacter character)
        {
            character.LooseHp(FlowerDamage);

            Destroy(gameObject);
        }
    }
}