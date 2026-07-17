using GameplaySystemsAndTools.Shared.Gameplay.Combat;

namespace GameplaySystemsAndTools.Features.Enemy
{
    public class EnemyAttackCooldownState : EnemyBaseState
    {
        private float coolDownTimer = 0f;
        private EnemyCombatDataSo combatData;

        public EnemyAttackCooldownState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            combatData = stateMachine.EnemyConfigSo.CombatData;
            
            stateMachine.Animator.CrossFadeInFixedTime(combatData.CombatIdleAnimHash,
                combatData.CrossFadeDurationCombat);
            coolDownTimer = combatData.AttackCoolDown;
        }

        public override void Tick(float deltaTime)
        {
            HandleBlocking(deltaTime, true);

            RotateToPlayer(deltaTime);

            coolDownTimer -= deltaTime;
            if (coolDownTimer <= 0f)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}