using System;
using GameplaySystemsAndTools.Shared.Events;
using TMPro;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.Season
{
    public class SeasonDisplayUI : MonoBehaviour
    {
        private EventBinding<SeasonChangedEvent> seasonChangedBinding;
        [SerializeField] private TextMeshProUGUI seasonText;

        private void OnEnable()
        {
            seasonChangedBinding = new EventBinding<SeasonChangedEvent>(HandleSeasonChangedEvent);
            EventBus<SeasonChangedEvent>.Subscribe(seasonChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<SeasonChangedEvent>.Unsubscribe(seasonChangedBinding);
        }

        private void HandleSeasonChangedEvent(SeasonChangedEvent evt)
        {
            seasonText.text = evt.CurrentSeasonType.ToString();
        }
    }
}