using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay
{
    public interface IKnockable
    {
        void ApplyKnockback(float force,Vector3 direction);
    }
}