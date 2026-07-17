using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Health
{
    public abstract class HurtboxBase : MonoBehaviour, IDamageable
    {
        protected HealthBase OwnerHealth;

        protected virtual void Awake()
        {
            if (!OwnerHealth)
                OwnerHealth = GetComponentInParent<HealthBase>();
        }

        public virtual void ApplyDamage(DamageInfo damageInfo)
        {
            OwnerHealth.ApplyDamage(damageInfo);
        }

        protected abstract void OnHitApplied(float finalDamage);
    }
}