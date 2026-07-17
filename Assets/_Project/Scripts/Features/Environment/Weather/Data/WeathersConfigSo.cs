using UnityEngine;

namespace GameplaySystemsAndTools.Features.Environment.Weather
{
    [CreateAssetMenu(fileName = "WeatherDataSo", menuName = "Scriptable Objects/Weather/WEather Config")]
    public class WeathersConfigSo : ScriptableObject
    {
        public ParticleSystem rainParticlePrefab;
        public ParticleSystem snowParticlePrefab;

        // Fully qualified: the Features.Environment.Skybox namespace shadows UnityEngine.Skybox here.
        public UnityEngine.Skybox skybox;
    }
}