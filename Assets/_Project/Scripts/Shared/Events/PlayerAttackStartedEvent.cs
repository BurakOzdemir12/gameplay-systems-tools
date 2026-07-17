using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Events
{
    /// <summary>
    /// Raised the moment an attack swing starts, so defensive AI (block/parry brains)
    /// can react. Replaces the old static C# event on PlayerAttackSignal.
    /// </summary>
    public struct PlayerAttackStartedEvent : IEvent
    {
        public GameObject Attacker;
        public GameObject Target;
        public AttackDataSo AttackData;

        public PlayerAttackStartedEvent(GameObject attacker, GameObject target, AttackDataSo attackData)
        {
            Attacker = attacker;
            Target = target;
            AttackData = attackData;
        }
    }
}
