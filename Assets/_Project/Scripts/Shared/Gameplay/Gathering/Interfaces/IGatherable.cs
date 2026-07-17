using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.Gathering
{
    public interface IGatherable
    {
        Transform InteractTransform { get; }
        // GameObject RequiredTool { get; }
    }
}