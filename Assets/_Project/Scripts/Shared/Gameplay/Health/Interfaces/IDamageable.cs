
namespace GameplaySystemsAndTools.Shared.Gameplay.Health
{
    public interface IDamageable
    {
        // void ApplyDamage(float damage);
        void ApplyDamage(DamageInfo damageInfo);
    }
}