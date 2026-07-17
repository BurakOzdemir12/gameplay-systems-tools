using GameplaySystemsAndTools.Shared.Data;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Events
{
    public struct CharacterCombatActionEvent : IEvent
    {
        public GameObject Source;
        public SurfaceType Surface;
        public CombatActionType Type;
        public WeaponType WeaponType;
        public string ActionTag;
        public Vector3 Position;

        public CharacterCombatActionEvent(GameObject source, CombatActionType type, WeaponType weaponType,
            SurfaceType surface, Vector3 position,
            string actionTag = "")
        {
            Source = source;
            Type = type;
            WeaponType = weaponType;
            Surface = surface;
            Position = position;
            ActionTag = actionTag;
        }
    }
}