using GameplaySystemsAndTools.Shared.Audio;
using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.TimeOfDay
{
    [CreateAssetMenu(fileName = "NewTimeConfig", menuName = "Scriptable Objects/Time/Time Config")]
    public class TimeConfigSo : ScriptableObject
    {
        public DivisionsOfDay defaultDivision;
        public float startHour;
        public float sunriseHour;
        public float sunsetHour;
        public float timeMultiplier;

    }
}