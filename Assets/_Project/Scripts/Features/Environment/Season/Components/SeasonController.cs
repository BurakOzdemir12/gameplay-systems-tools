using System;
using GameplaySystemsAndTools.Shared.Events;
using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.Season
{
    public class SeasonController : MonoBehaviour
    {
        [SerializeField] private SeasonConfigSo seasonData;
        private EventBinding<DayChangedEvent> dayChangedBinding;
        private SeasonType currentSeasonType;
        private int daysPassedInCurrentSeason;

        private void Awake()
        {
            currentSeasonType = SeasonType.Spring;
            daysPassedInCurrentSeason = 0;
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
            EventBus<SeasonChangedEvent>.Publish(new SeasonChangedEvent(SeasonType.Spring));
        }

        private void HandleDayChangedEvent(DayChangedEvent evt)
        {
            daysPassedInCurrentSeason++;
            if (daysPassedInCurrentSeason >= seasonData.daysPerSeason)
            {
                daysPassedInCurrentSeason = 0;
                SwitchToNextSeason();
            }
        }

        private void SwitchToNextSeason()
        {
            int nextSeasonIndex = ((int)currentSeasonType + 1) % 4;
            currentSeasonType = (SeasonType)nextSeasonIndex;
            EventBus<SeasonChangedEvent>.Publish(new SeasonChangedEvent(currentSeasonType));
        }
    }
}