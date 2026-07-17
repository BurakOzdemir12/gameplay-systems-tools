using System;
using GameplaySystemsAndTools.Shared.Data;

namespace GameplaySystemsAndTools.Shared.Events
{
    public struct TimeChangedEvent : IEvent
    {
        public DateTime TimeOfDay;
        public DivisionsOfDay Division;
        public bool IsDivisionJustChanged;

        public TimeChangedEvent(DateTime timeOfDay, DivisionsOfDay division, bool ısDivisionJustChanged)
        {
            TimeOfDay = timeOfDay;
            Division = division;
            IsDivisionJustChanged = ısDivisionJustChanged;
        }
    }
}