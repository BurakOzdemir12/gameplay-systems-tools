using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Audio
{
    /// <summary>
    /// Injection-friendly audio contract. Prefer publishing SoundPlayRequestedEvent from
    /// per-instance gameplay code; inject this only where a direct call is clearer.
    /// </summary>
    public interface IAudioService
    {
        void PlayGeneric3DSound(AudioClip clip, Vector3 position, SoundChannel channel, float volume = 1f,
            bool isFrequent = true, bool isLoop = false);
        void StopAllSounds();
    }
}
