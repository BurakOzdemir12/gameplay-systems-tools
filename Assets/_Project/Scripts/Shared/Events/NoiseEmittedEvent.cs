using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Events
{
    /// <summary>
    /// Raised by anything that makes an audible gameplay noise (thrown rocks, impacts).
    /// NoiseService listens and forwards the stimulus to INoiseListener sensors in range.
    /// Spawn-safe: runtime-instantiated objects can publish without needing injection.
    /// </summary>
    public struct NoiseEmittedEvent : IEvent
    {
        public Vector3 Position;
        public float Radius;
        public GameObject Source;
        public LayerMask ListenerLayers;

        public NoiseEmittedEvent(Vector3 position, float radius, GameObject source, LayerMask listenerLayers)
        {
            Position = position;
            Radius = radius;
            Source = source;
            ListenerLayers = listenerLayers;
        }
    }
}
