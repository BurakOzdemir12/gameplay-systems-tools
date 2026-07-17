using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    public interface IBlocker
    {
        void ApplyBlock(BlockContext context);
        bool CanBlock(Transform attackerRoot);
        bool IsBlocking { get; }
    }
}