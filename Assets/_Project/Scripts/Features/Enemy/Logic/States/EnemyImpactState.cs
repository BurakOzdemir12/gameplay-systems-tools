using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using UnityEngine;

namespace GameplaySystemsAndTools.Features.Enemy
{
    public class EnemyImpactState : EnemyBaseState
    {
        private float remainingImpactTime;
        private EnemyCombatDataSo combatData;

        public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            combatData = stateMachine.EnemyConfigSo.CombatData;

            remainingImpactTime = combatData.ImpactDuration;

            stateMachine.Animator.CrossFadeInFixedTime(combatData.ImpactSlightAnimHash,
                combatData.CrossFadeDurationCombat);

            stateMachine.EnemyPerceptionController.IsAggressive = true;
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            remainingImpactTime -= deltaTime;

            if (remainingImpactTime <= 0)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}