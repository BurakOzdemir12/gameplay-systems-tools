using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Audio
{
    [CreateAssetMenu(fileName = "GameMusicProfile", menuName = "Scriptable Objects/Audio Profile/Game Music Profile")]
    public class GameMusicProfile : ScriptableObject
    {
        [Header("Ambient Tracks by place")] public AudioClip[] dayVillageTracks;
        [Header("Exploration Ambient Tracks")] public AudioClip[] dayExplorationTracks;
        public AudioClip[] nightExplorationTracks;
        [Header("StateBase tracks")] public AudioClip[] dangerTracks;
        public AudioClip[] bossTracks;

        public AudioClip GetMusicTrack(bool isDanger, DivisionsOfDay time)
        {
            if (isDanger) return GetRandom(dangerTracks);

            return time switch
            {
                DivisionsOfDay.Night or DivisionsOfDay.Evening => GetRandom(nightExplorationTracks),
                _ => GetRandom(dayExplorationTracks)
            };
        }

        private AudioClip GetRandom(AudioClip[] clips)
        {
            if (clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }
    }
}