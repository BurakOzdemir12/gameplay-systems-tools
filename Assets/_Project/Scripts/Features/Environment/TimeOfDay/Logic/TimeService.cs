using System;
using GameplaySystemsAndTools.Shared.Data;
using GameplaySystemsAndTools.Shared.Events;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.TimeOfDay
{
    /// <summary>
    /// Pure C# day/night clock. Advances an in-game DateTime, computes sun/moon angles
    /// and publishes TimeChangedEvent / DayChangedEvent on the EventBus when the hour
    /// or day rolls over. Ticked by TimeOfDayController; consumed via ITimeOfDayService.
    /// </summary>
    public class TimeService : ITimeOfDayService
    {
        private readonly TimeConfigSo timeData;

        private DateTime currentTime;
        public DateTime CurrentTime => currentTime;

        private readonly TimeSpan sunriseTime;
        private readonly TimeSpan sunsetTime;

        private bool isDayTime;
        private int currentHour;

        // Cached values so change events only fire on real transitions.
        private DivisionsOfDay lastDivision;
        private int lastHour;
        private int lastDay;

        public TimeService(TimeConfigSo timeData)
        {
            this.timeData = timeData;

            currentTime = new DateTime(1, 1, 1) + TimeSpan.FromHours(timeData.startHour);

            sunriseTime = TimeSpan.FromHours(timeData.sunriseHour);
            sunsetTime = TimeSpan.FromHours(timeData.sunsetHour);

            lastDay = currentTime.Day;
            lastHour = currentTime.Hour;

            lastDivision = CalculateCurrentDivision();
        }

        public void UpdateTime(float deltaTime)
        {
            currentTime = currentTime.AddSeconds(deltaTime * timeData.timeMultiplier);

            isDayTime = IsDayTime();
            currentHour = currentTime.Hour;

            if (currentTime.Day != lastDay)
            {
                lastDay = currentTime.Day;
                EventBus<DayChangedEvent>.Publish(new DayChangedEvent(lastDay));
            }

            if (lastHour != currentHour)
            {
                DivisionsOfDay currentDivision = CalculateCurrentDivision();
                bool hasDivisionChanged = currentDivision != lastDivision;

                lastHour = currentHour;
                EventBus<TimeChangedEvent>.Publish(
                    new TimeChangedEvent(currentTime, currentDivision, hasDivisionChanged));

                lastDivision = currentDivision;
            }
        }

        public bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

        private float GetTimeProgress(TimeSpan start, TimeSpan end)
        {
            TimeSpan totalTime = CalculateDifference(start, end);
            TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

            return (float)(elapsedTime.TotalMinutes / totalTime.TotalMinutes);
        }

        // Sun sweeps 0-180 degrees during the day, 180-360 at night (below horizon).
        public float GetSunRotation()
        {
            if (isDayTime)
            {
                float progress = GetTimeProgress(sunriseTime, sunsetTime);
                return Mathf.Lerp(0, 180, progress);
            }
            else
            {
                float progress = GetTimeProgress(sunsetTime, sunriseTime);
                return Mathf.Lerp(180, 360, progress);
            }
        }

        public float GetMoonRotation()
        {
            if (!isDayTime)
            {
                float progress = GetTimeProgress(sunsetTime, sunriseTime);
                return Mathf.Lerp(0, 180, progress);
            }
            else
            {
                float progress = GetTimeProgress(sunriseTime, sunsetTime);
                return Mathf.Lerp(180, 360, progress);
            }
        }

        private TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
        {
            TimeSpan difference = to - from;

            // Wrap across midnight so "22:00 -> 06:00" is 8 hours, not negative.
            return difference.TotalHours < 0 ? difference + TimeSpan.FromDays(1) : difference;
        }

        private DivisionsOfDay CalculateCurrentDivision()
        {
            TimeSpan time = currentTime.TimeOfDay;
            if (time < sunriseTime) return DivisionsOfDay.Night;
            if (time < TimeSpan.FromHours(12)) return DivisionsOfDay.Morning;
            if (time < sunsetTime) return DivisionsOfDay.Afternoon;
            return time < TimeSpan.FromHours(22) ? DivisionsOfDay.Evening : DivisionsOfDay.Night;
        }
    }
}
