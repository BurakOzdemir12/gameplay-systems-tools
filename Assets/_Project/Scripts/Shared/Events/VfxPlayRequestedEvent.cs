using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Events
{
    /// <summary>
    /// Request for a one-shot visual effect. VfxService listens and spawns the prefab.
    /// Same decoupling idea as SoundPlayRequestedEvent.
    /// </summary>
    public struct VfxPlayRequestedEvent : IEvent
    {
        public GameObject VfxPrefab;
        public Vector3 Position;
        public Quaternion Rotation;

        public VfxPlayRequestedEvent(GameObject vfxPrefab, Vector3 position, Quaternion rotation)
        {
            VfxPrefab = vfxPrefab;
            Position = position;
            Rotation = rotation;
        }
    }
}
