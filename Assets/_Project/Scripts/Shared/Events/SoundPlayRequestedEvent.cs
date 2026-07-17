using GameplaySystemsAndTools.Shared.Audio;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Events
{
    /// <summary>
    /// Request for a one-shot 3D sound. AudioService listens and plays it through the
    /// pooled emitters. Lets per-instance gameplay code (enemy states, shields) trigger
    /// audio without holding a reference to the audio system.
    /// </summary>
    public struct SoundPlayRequestedEvent : IEvent
    {
        public AudioClip Clip;
        public Vector3 Position;
        public SoundChannel Channel;
        public float Volume;
        public bool IsFrequent;
        public bool IsLoop;

        public SoundPlayRequestedEvent(AudioClip clip, Vector3 position, SoundChannel channel,
            float volume = 1f, bool isFrequent = true, bool isLoop = false)
        {
            Clip = clip;
            Position = position;
            Channel = channel;
            Volume = volume;
            IsFrequent = isFrequent;
            IsLoop = isLoop;
        }
    }
}
