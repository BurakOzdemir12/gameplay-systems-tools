using GameplaySystemsAndTools.Features.Enemy;
using GameplaySystemsAndTools.Features.Environment.Skybox;
using GameplaySystemsAndTools.Features.Environment.TimeOfDay;
using GameplaySystemsAndTools.Features.Inventory;
using GameplaySystemsAndTools.Shared.Audio;
using GameplaySystemsAndTools.Shared.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameplaySystemsAndTools.Core
{
    /// <summary>
    /// Per-scene composition root for gameplay scenes. Discovers the scene's engine
    /// components once, registers them behind their contracts, then injects every
    /// scene-placed consumer through a build callback. Registrations are conditional
    /// so the same scope works in scenes that only contain a subset of the systems
    /// (e.g. an environment-only scene has no enemies or inventory).
    /// Runtime-spawned objects never rely on this — they use EventBus events.
    /// </summary>
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // --- scene service discovery (composition root only; gameplay code never Finds) ---
            var timeOfDay = FindAnyObjectByType<TimeOfDayController>(FindObjectsInactive.Include);
            if (timeOfDay != null)
            {
                builder.RegisterComponent(timeOfDay);
                builder.Register<ITimeOfDayService>(_ => timeOfDay.TimeService, Lifetime.Scoped);
            }

            var audio = FindAnyObjectByType<AudioService>(FindObjectsInactive.Include);
            if (audio != null)
            {
                builder.RegisterComponent(audio).As<IAudioService>();
            }

            var hudPool = FindAnyObjectByType<EnemyHUDPool>(FindObjectsInactive.Include);
            if (hudPool != null)
            {
                builder.RegisterComponent(hudPool);
            }

            var input = FindAnyObjectByType<PlayerInputHandler>(FindObjectsInactive.Include);
            if (input != null)
            {
                builder.RegisterComponent(input);
            }

            // --- inject scene-placed consumers (supports multiple instances per type) ---
            builder.RegisterBuildCallback(container =>
            {
                if (timeOfDay != null)
                {
                    InjectAll<TimeDisplayUI>(container);
                    InjectAll<SkyboxController>(container);
                }

                if (hudPool != null)
                {
                    InjectAll<EnemyUIController>(container);
                }

                if (input != null)
                {
                    InjectAll<InventoryComponent>(container);
                    InjectAll<PickupController>(container);
                }
            });
        }

        private static void InjectAll<T>(IObjectResolver container) where T : MonoBehaviour
        {
            var components = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var component in components)
            {
                container.Inject(component);
            }
        }
    }
}
