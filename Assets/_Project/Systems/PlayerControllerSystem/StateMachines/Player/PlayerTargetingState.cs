using _Project.Systems.PlayerControllerSystem.StateMachines.Player;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerTargetingState : PlayerBaseState
    {
        public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            stateMachine.InputHandler.TargetCancelEvent += OnTargetCancel;
            stateMachine.Animator.Play(stateMachine.TargetingBlendTreeHash);
        }

        public override void Tick(float deltaTime)
        {
            if (stateMachine.Targeter.SelectedTarget == null)
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                return;
            }

            FaceTarget(stateMachine.Targeter.SelectedTarget, deltaTime);

            Vector3 movement = CalculateMovement();
            Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
            Debug.Log(stateMachine.Targeter.SelectedTarget.name);
        }

        public override void Exit()
        {
            stateMachine.InputHandler.TargetCancelEvent -= OnTargetCancel;
        }

        private void OnTargetCancel()
        {
            stateMachine.Targeter.DeselectTarget();

            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }

        private Vector3 CalculateMovement()
        {
            Vector3 movement = new Vector3();
            movement += stateMachine.transform.right * stateMachine.InputHandler.Move.x;
            movement += stateMachine.transform.forward * stateMachine.InputHandler.Move.y;

            return movement;
        }
    }
}