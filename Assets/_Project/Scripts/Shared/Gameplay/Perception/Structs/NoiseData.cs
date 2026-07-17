using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Perception
{
    public struct NoiseData
    {
        public Vector3 Position;
        public GameObject Source;
        public float Range;
        public PerceptionType Type;

        public NoiseData(Vector3 position, GameObject source, float range, PerceptionType type)
        {
            Position = position;
            Source = source;
            Range = range;
            Type = type;
        }
    }
}