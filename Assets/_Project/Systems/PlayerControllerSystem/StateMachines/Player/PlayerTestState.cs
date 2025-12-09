using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerTestState : PlayerBaseState
    {
        private float timer = 5f;

        public PlayerTestState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.InputHandler.JumpEvent += OnJump;

            Debug.Log("Entered Test State, state will change after " + timer + " seconds");
        }

        public override void Tick(float deltaTime)
        {
            timer -= deltaTime;
            // Debug.Log("Tick " + timer);

            if (timer <= 0)
            {
                stateMachine.SwitchState(new PlayerTestState(this.stateMachine));
            }
        }

        public override void Exit()
        {
            stateMachine.InputHandler.JumpEvent -= OnJump;

            Debug.Log("Exited Test State");
        }

        private void OnJump()
        {
            stateMachine.SwitchState(new PlayerTestState(stateMachine));
        }
    }
}