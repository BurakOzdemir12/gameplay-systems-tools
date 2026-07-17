using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Events
{
    public struct CharacterGatheringActionEvent : IEvent
    {
        public GameObject Source;
        public GatherActionType Type;
        // public SurfaceType Surface;
        public ToolType ToolType;
        public string ActionTag;
        public Vector3 Position;

        public CharacterGatheringActionEvent(GameObject source, GatherActionType type,
            ToolType toolType, Vector3 position = default, string actionTag = "") //SurfaceType surface,
        {
            Type = type;
            // Surface = surface;
            Source = source;
            ToolType = toolType;
            ActionTag = actionTag;
            Position = position;
        }
    }
}