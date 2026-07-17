using GameplaySystemsAndTools.Shared.Events;
using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Player
{
    /// <summary>
    /// Thin publisher: announces the start of a player attack swing on the EventBus so
    /// any interested system (enemy defence brains, analytics) can react without a
    /// direct reference to the player. Called from attack states / animation events.
    /// </summary>
    public sealed class PlayerAttackSignal : MonoBehaviour
    {
        public void RaiseAttack(GameObject explicitTarget, AttackDataSo attackDataSo)
        {
            EventBus<PlayerAttackStartedEvent>.Publish(
                new PlayerAttackStartedEvent(gameObject, explicitTarget, attackDataSo));
        }
    }
}
