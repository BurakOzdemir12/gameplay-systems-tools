using GameplaySystemsAndTools.Shared.Data;

namespace GameplaySystemsAndTools.Shared.Events
{
    public struct SeasonChangedEvent : IEvent
    {
        public SeasonType CurrentSeasonType;

        public SeasonChangedEvent(SeasonType currentSeasonType)
        {
            CurrentSeasonType = currentSeasonType;
        }
    }
}