using UnityEngine;
using UnityEngine.Audio;

namespace GameplaySystemsAndTools.Shared.Audio
{
    public struct SoundData
    {
        public AudioClip Clip;
        public Vector3 Position;
        public AudioMixerGroup MixerGroup;
        public float Volume;
        public float Pitch;
        public float SpatialBlend;
        public bool FrequentSound;
        public bool Loop;

        public SoundData(AudioClip clip, Vector3 position, AudioMixerGroup mixerGroup, float volume, float pitch,
            float spatialBlend,
            bool frequentSound, bool loop)
        {
            Clip = clip;
            Position = position;
            MixerGroup = mixerGroup;
            Volume = volume;
            Pitch = pitch;
            SpatialBlend = spatialBlend;
            FrequentSound = frequentSound;
            Loop = loop;
        }
    }
}