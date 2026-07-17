using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Gathering
{
    public class ResourceNode : MonoBehaviour, IGatherable
    {
        [SerializeField] private Transform interactPoint;

        public Transform InteractTransform => interactPoint != null
            ? interactPoint
            : transform;
    }
}