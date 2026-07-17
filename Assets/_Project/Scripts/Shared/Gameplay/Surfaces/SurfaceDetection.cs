using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Surfaces
{
    public class SurfaceDetection : MonoBehaviour
    {
        [SerializeField] private LayerMask surfaceMask = ~0;
        [SerializeField] private float rayDistance = 1.2f;
        [SerializeField] private SurfaceType fallback = SurfaceType.Dirt;

        public SurfaceType GetSurfaceData(Vector3 worldPos)
        {
            Vector3 origin = worldPos + Vector3.up * 0.1f;
            if (Physics.Raycast(origin, Vector3.down, out var hitData, rayDistance, surfaceMask)
               )
            {
                var def = hitData.collider.GetComponentInParent<SurfaceDefinition>();
                if (def != null)
                {
                    return def.SurfaceType;
                }
            }

            return fallback;
        }
    }
}