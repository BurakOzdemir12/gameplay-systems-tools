using GameplaySystemsAndTools.Shared.Events;
using GameplaySystemsAndTools.Shared.Gameplay.Perception;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay
{
    /// <summary>
    /// A physics prop that makes noise on impact (e.g. a thrown rock used to distract
    /// enemies). Publishes NoiseEmittedEvent instead of calling a singleton, because
    /// throwables are spawned at runtime and must not rely on scene injection.
    /// </summary>
    public class Throwable : MonoBehaviour
    {
        [Header("Settings")] [Tooltip("Who is gonna hear the noise => enemy")] [SerializeField]
        private LayerMask listenerLayers;

        [Tooltip("Noise radius when the object hits something")] [SerializeField]
        private float baseNoiseRadius = 5f;

        [Tooltip("Minimum impact velocity required to make a sound")] [SerializeField]
        private float minVelocityToMakeNoise = 1f;

        [Tooltip("Object will be destroyed after this time")] [SerializeField]
        private float destroyTime = 15f;

        private Vector3 debugFirstContactPoint;
        private bool hasAlreadyCollided = false;

        private void Start()
        {
            Destroy(gameObject, destroyTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasAlreadyCollided) return;

            float velocity = collision.relativeVelocity.magnitude;

            if (velocity > minVelocityToMakeNoise)
            {
                // Louder throws are heard farther away, clamped so it stays sane.
                float finalRadius = baseNoiseRadius * Mathf.Clamp(velocity * 0.5f, 0.5f, 2f);

                EventBus<NoiseEmittedEvent>.Publish(
                    new NoiseEmittedEvent(collision.contacts[0].point, finalRadius, gameObject, listenerLayers));
#if UNITY_EDITOR
                debugFirstContactPoint = collision.contacts[0].point;
#endif
                hasAlreadyCollided = true;
            }

            CheckInteraction(collision.collider);
        }

        // If it touches any character that can hear noise, the prop consumed its purpose
        // (the distraction landed) and removes itself.
        private void CheckInteraction(Collider other)
        {
            if (((1 << other.gameObject.layer) & listenerLayers) == 0) return;
            Transform rootTarget = other.transform.root;
            if (rootTarget.TryGetComponent<INoiseListener>(out _))
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(debugFirstContactPoint, baseNoiseRadius);
        }
    }
}
