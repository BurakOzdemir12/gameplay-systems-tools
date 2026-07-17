using System;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Combat
{
    public class Target : MonoBehaviour
    {
        public event Action<Target> OnTargetDestroyed;

        private void OnDestroy()
        {
            OnTargetDestroyed?.Invoke(this);
        }
    }
}