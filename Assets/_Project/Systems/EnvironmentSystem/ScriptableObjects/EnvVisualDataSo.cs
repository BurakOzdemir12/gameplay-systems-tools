using _Project.Systems.EnvironmentSystem.Time.Enums;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnvAmbientDataSo", menuName = "Scriptable Objects/Env & Visuals/Env Config")]
    public class EnvVisualDataSo : ScriptableObject
    {
        [Header("Audio Settings by Time")] public AudioClip daySound;
        public AudioClip nightSound;
        [Header("Audio Settings by Weather")] public AudioClip rainSound;
        public AudioClip hardRainSound;
        public AudioClip snowSound;
        [Header("Audio Settings by Location")] public AudioClip indoorSound;
        public AudioClip forestSound;
        public AudioClip caveSound;
        [Header("Audio Settings by Location")] public AudioClip[] nightAmbientTracks;
        public AudioClip[] lightAmbientTracks;
        public AudioClip[] actionAmbientTracks;

        public void PlayRandomAmbientTrack(DivisionsOfDay divisionOfDay)
        {
            switch (divisionOfDay)
            {
                case DivisionsOfDay.Morning:
                    break;
            }
        }
    }
}