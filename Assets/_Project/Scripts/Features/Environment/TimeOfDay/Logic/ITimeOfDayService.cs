using System;

namespace GameplaySystemsAndTools.Features.Environment.TimeOfDay
{
    /// <summary>
    /// Read-only clock contract consumed by systems that follow the day/night cycle
    /// (skybox, UI, audio ambience). Resolved through VContainer, never via singleton.
    /// </summary>
    public interface ITimeOfDayService
    {
        DateTime CurrentTime { get; }
        bool IsDayTime();
        float GetSunRotation();
        float GetMoonRotation();
    }
}
