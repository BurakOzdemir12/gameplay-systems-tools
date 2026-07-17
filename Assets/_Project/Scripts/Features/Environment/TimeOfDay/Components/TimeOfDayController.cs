using UnityEngine;
using UnityEngine.InputSystem;

namespace GameplaySystemsAndTools.Features.Environment.TimeOfDay
{
    /// <summary>
    /// Scene-side driver of the day/night clock: owns the TimeConfigSo, creates the
    /// TimeService and ticks it every frame. GameplayLifetimeScope registers the
    /// service it exposes as ITimeOfDayService for all consumers.
    /// </summary>
    public class TimeOfDayController : MonoBehaviour
    {
        [SerializeField] private TimeConfigSo timeConfig;
        public TimeConfigSo TimeConfig => timeConfig;

        private TimeService timeService;

        // Lazy creation keeps this safe to resolve from the DI scope regardless of
        // Awake execution order between this component and the LifetimeScope.
        public TimeService TimeService => timeService ??= new TimeService(timeConfig);

        private void Update()
        {
            TimeService.UpdateTime(Time.deltaTime);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            HandleDebugTimeScaleKeys();
#endif
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Dev-only cheat: L doubles, M halves the in-game clock speed.
        private void HandleDebugTimeScaleKeys()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                timeConfig.timeMultiplier *= 2;
            }

            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                timeConfig.timeMultiplier /= 2;
            }
        }
#endif
    }
}
