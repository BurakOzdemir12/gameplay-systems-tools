using GameplaySystemsAndTools.Shared.Events;
using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameplaySystemsAndTools.Features.Enemy
{
    /// <summary>
    /// Decides whether this enemy blocks or parries an incoming player attack.
    /// Listens to PlayerAttackStartedEvent on the EventBus (no direct player reference)
    /// and scores block/parry chances from the enemy's AI brain data vs. the attack data.
    /// </summary>
    public class EnemyDefenceBrain : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine stateMachine;

        public bool canBlockAttack = false;
        public bool canParryAttack = false;

        private EventBinding<PlayerAttackStartedEvent> attackStartedBinding;

        private void Awake()
        {
            if (!stateMachine)
            {
                stateMachine = GetComponent<EnemyStateMachine>();
            }
        }

        private void OnEnable()
        {
            attackStartedBinding = new EventBinding<PlayerAttackStartedEvent>(OnPlayerAttackStarted);
            EventBus<PlayerAttackStartedEvent>.Subscribe(attackStartedBinding);
        }

        private void OnDisable()
        {
            EventBus<PlayerAttackStartedEvent>.Unsubscribe(attackStartedBinding);
        }

        private void OnPlayerAttackStarted(PlayerAttackStartedEvent evt)
        {
            if (!evt.Attacker) return;

            // Only react if this enemy is actually aware of the attacker.
            bool isPerceived = stateMachine.EnemyPerceptionController.IsPerceivingTarget(evt.Attacker);
            if (!isPerceived) return;

            DecideDefenceAction(evt.AttackData);
        }

        private void DecideDefenceAction(AttackDataSo attackData)
        {
            if (!stateMachine.ShieldHandler.CurrentShieldLogic)
            {
                canBlockAttack = false;
                canParryAttack = false;
                return;
            }

            float attackScore = attackData.attackScore;
            EnemyAIBrainDataSo brainData = stateMachine.EnemyConfigSo.AIBrainData;

            // Chance formula: defence score vs attack score, clamped to [0,1].
            float blockScore = brainData.blockAttackScore;
            float blockChance = Mathf.Clamp01(blockScore / (blockScore + attackScore));
            canBlockAttack = Random.value < blockChance;

            float parryScore = brainData.parryAttackScore;
            float parryChance = Mathf.Clamp01(parryScore / (parryScore + attackScore));
            canParryAttack = Random.value < parryChance;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"Block chance: {blockChance} -> {canBlockAttack} | Parry chance: {parryChance} -> {canParryAttack}");
#endif

            SetEnemyState();
        }

        private void SetEnemyState()
        {
            if (stateMachine.CurrentState is EnemyDeadState or EnemyImpactState or EnemyAttackingState)
                return;

            if (!stateMachine.ShieldHandler.CurrentShieldLogic)
            {
                canParryAttack = false;
                return;
            }

            if (canParryAttack)
            {
                stateMachine.SwitchState(new EnemyParryState(stateMachine));
            }
        }
    }
}
