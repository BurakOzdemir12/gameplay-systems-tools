using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Surfaces
{
    public class SurfaceDefinition : MonoBehaviour
    {
        [SerializeField] private SurfaceType surfaceType;
        public SurfaceType SurfaceType => surfaceType;
    }
}
