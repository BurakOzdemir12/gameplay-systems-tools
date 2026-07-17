using System.Collections.Generic;
using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.Season
{
    [CreateAssetMenu(fileName = "SeasonDataSo", menuName = "Scriptable Objects/Season/Season Config")]
    public class SeasonConfigSo : ScriptableObject
    {
        public int daysPerSeason;

        public List<WeatherType> springWeathers;
        public List<WeatherType> summerWeathers;
        public List<WeatherType> autumnWeathers;
        public List<WeatherType> winterWeathers;
    }
}