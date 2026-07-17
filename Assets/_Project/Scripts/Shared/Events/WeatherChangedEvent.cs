using GameplaySystemsAndTools.Shared.Data;

namespace GameplaySystemsAndTools.Shared.Events
{
    public struct WeatherChangedEvent : IEvent
    {
        public WeatherType CurrentWeatherType;

        public WeatherChangedEvent(WeatherType currentWeatherType)
        {
            CurrentWeatherType = currentWeatherType;
        }
    }
}