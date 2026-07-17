using GameplaySystemsAndTools.Shared.Data;
using GameplaySystemsAndTools.Shared.Gameplay.Items;
using GameplaySystemsAndTools.Shared.Gameplay.Feedback;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon/Weapon Data")]
    public class WeaponData : ItemData
    {
        public WeaponImpactFeedbackProfile weaponImpactFeedbackProfile;
        public WeaponType weaponType;

        public GameObject trailPrefab;
        public float damage;
        public float knockback;
        public float staminaCost;
        public float durability;
    }
}