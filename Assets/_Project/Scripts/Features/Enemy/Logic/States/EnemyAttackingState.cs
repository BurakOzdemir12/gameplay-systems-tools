using GameplaySystemsAndTools.Shared.Gameplay.Combat;
using GameplaySystemsAndTools.Shared.Data;

namespace GameplaySystemsAndTools.Features.Enemy
{
    public class EnemyAttackingState : EnemyBaseState
    {
        public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string ATTACK_TAG = "Attack";
        private EnemyMovementDataSo movementData;
        private EnemyCombatDataSo combatData;

        public override void Enter()
        {
            movementData = stateMachine.EnemyConfigSo.MovementData;
            combatData = stateMachine.EnemyConfigSo.CombatData;

            stateMachine.EnemyPerceptionController.IsAggressive = true;

            float finalDamage = combatData.AttackDamage;
            float finalKnockbackForce = combatData.AttackKnockBackForce;

            stateMachine.WeaponHandler.CurrentWeaponLogic.SetupAttack(finalDamage, finalKnockbackForce, "normal");

            stateMachine.Animator.SetFloat(movementData.FreeLookSpeedParamHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(combatData.EnemyAttack1RHash,
                combatData.CrossFadeDurationCombat);
        }


        public override void Tick(float deltaTime)
        {
            Move(deltaTime);
            RotateToPlayer(deltaTime);

            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, ATTACK_TAG);
            if (normalizedTime >= 0.9f)
            {
                stateMachine.SwitchState(new EnemyAttackCooldownState(stateMachine));
            }
        }

        public override void Exit()
        {
            stateMachine.WeaponHandler.CurrentWeaponLogic.EndAttack();
        }
    }
}