using GameplaySystemsAndTools.Shared.Events;
using TMPro;
using UnityEngine;
using VContainer;

namespace GameplaySystemsAndTools.Features.Environment.TimeOfDay
{
    /// <summary>
    /// HUD clock: shows the in-game time and day. Gets the clock via VContainer
    /// injection (wired by GameplayLifetimeScope) instead of a singleton lookup.
    /// </summary>
    public class TimeDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dayText;

        private EventBinding<DayChangedEvent> dayChangedBinding;
        private ITimeOfDayService timeService;
        private int lastMinute = -1;

        [Inject]
        public void Construct(ITimeOfDayService timeOfDayService)
        {
            timeService = timeOfDayService;
        }

        private void OnEnable()
        {
            dayChangedBinding = new EventBinding<DayChangedEvent>(HandleDayChangedEvent);
            EventBus<DayChangedEvent>.Subscribe(dayChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<DayChangedEvent>.Unsubscribe(dayChangedBinding);
        }

        private void Start()
        {
            if (timeService == null) return;
            dayText.text = $"Day: {timeService.CurrentTime.Day}";
            timeText.text = timeService.CurrentTime.ToString("HH:mm");
        }

        private void Update()
        {
            if (timeService == null) return;
            UpdateUI();
        }

        private void HandleDayChangedEvent(DayChangedEvent evt)
        {
            dayText.text = evt.CurrentDay.ToString();
        }

        private void UpdateUI()
        {
            // Only touch the text when the displayed minute actually changes (no per-frame string churn).
            int currentMinute = timeService.CurrentTime.Minute;
            if (currentMinute == lastMinute) return;

            lastMinute = currentMinute;

            timeText.text = timeService.CurrentTime.ToString("HH:mm");
        }
    }
}
