using GameplaySystemsAndTools.Shared.Events;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Perception
{
    /// <summary>
    /// Turns gameplay noises into perception stimuli: listens to NoiseEmittedEvent,
    /// overlaps the noise radius and notifies every INoiseListener hit.
    /// Scene service registered in the GameplayLifetimeScope (no more static Instance).
    /// </summary>
    public class NoiseService : MonoBehaviour
    {
        // Non-alloc buffer: noise bursts happen often, so avoid per-call allocations.
        private readonly Collider[] hitBuffer = new Collider[10];

        private EventBinding<NoiseEmittedEvent> noiseBinding;

        private void OnEnable()
        {
            noiseBinding = new EventBinding<NoiseEmittedEvent>(HandleNoiseEmitted);
            EventBus<NoiseEmittedEvent>.Subscribe(noiseBinding);
        }

        private void OnDisable()
        {
            EventBus<NoiseEmittedEvent>.Unsubscribe(noiseBinding);
        }

        private void HandleNoiseEmitted(NoiseEmittedEvent evt)
        {
            EmitNoise(evt.Position, evt.Radius, evt.Source, evt.ListenerLayers);
        }

        public void EmitNoise(Vector3 position, float radius, GameObject source, LayerMask listenerLayers)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(position, radius, hitBuffer, listenerLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                if (hitBuffer[i].TryGetComponent<INoiseListener>(out var listener))
                {
                    listener.OnNoiseDetected(new NoiseData(position, source, radius, PerceptionType.Stimulus));
                }
            }
        }
    }
}
