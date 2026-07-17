using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    public readonly struct BlockContext
    {
        public readonly Transform AttackerRoot;
        public readonly float Knockback;
        public readonly float Damage;
        public readonly string AttackType;

        public BlockContext(float damage, float knockback, Transform attackerRoot, string attackType)
        {
            Damage = damage;
            Knockback = knockback;
            AttackerRoot = attackerRoot;
            AttackType = attackType;
        }

    }
}