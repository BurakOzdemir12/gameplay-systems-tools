using System;
using GameplaySystemsAndTools.Shared.Events;
using TMPro;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.Weather
{
    public class WeatherDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weatherText;
        private EventBinding<WeatherChangedEvent> weatherChangedBinding;

        private void OnEnable()
        {
            weatherChangedBinding = new EventBinding<WeatherChangedEvent>(HandleWeatherChangedEvent);
            EventBus<WeatherChangedEvent>.Subscribe(weatherChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<WeatherChangedEvent>.Unsubscribe(weatherChangedBinding);
        }
        private void HandleWeatherChangedEvent(WeatherChangedEvent evt)
        {
            weatherText.text = evt.CurrentWeatherType.ToString();
        }

    }
}