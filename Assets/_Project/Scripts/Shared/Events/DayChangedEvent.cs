
namespace GameplaySystemsAndTools.Shared.Events
{
    public struct DayChangedEvent : IEvent
    {
        public int CurrentDay;
        // public bool IsDayJustChanged;

        public DayChangedEvent(int currentDay) //, bool ısDayJustChanged
        {
            CurrentDay = currentDay;
            // IsDayJustChanged = ısDayJustChanged;
        }
    }
}