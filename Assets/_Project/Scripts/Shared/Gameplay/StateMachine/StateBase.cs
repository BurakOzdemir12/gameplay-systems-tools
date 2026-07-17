using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.StateMachine
{
    /// <summary>
    /// Base node of the hierarchical FSM: Enter/Tick/Exit plus nested sub-states
    /// (SetSubState) and root switching (SwitchRootState). Actor-agnostic by design —
    /// each actor derives its own base state (PlayerBaseState, EnemyBaseState).
    /// </summary>
    public abstract class StateBase
    {
        protected readonly StateMachineBase stateMachine;

        protected StateBase superState;
        protected StateBase subState;

        protected StateBase(StateMachineBase stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public abstract void Enter();


        public abstract void Tick(float deltaTime);
        public abstract void Exit();

        public void UpdateStates(float deltaTime)
        {
            Tick(deltaTime);
            subState?.UpdateStates(deltaTime);
        }

        public StateBase GetLeafState()
        {
            StateBase s = this;
            while (s.subState != null) s = s.subState;
            return s;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected void SetSubState(StateBase newSubState)
        {
            if (subState != null)
            {
                stateMachine.PreviousLeafState = subState.GetLeafState();
            }
            else
            {
                stateMachine.PreviousLeafState = this;
            }

            subState?.Exit();
            subState = newSubState;
            subState.superState = this;
            subState.Enter();
        }

        protected void ClearSubState()
        {
            subState?.Exit();
            subState = null;
        }

        protected void SwitchRootState(StateBase newRootState)
        {
            stateMachine.SwitchState(newRootState);
        }

        public StateBase GetSubState() => subState;
        public StateBase GetSuperState() => superState;

        protected float GetNormalizedTime(Animator animator, int layerIndex, string tag)
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(layerIndex);

            if (animator.IsInTransition(layerIndex) && nextInfo.IsTag(tag))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(layerIndex) && currentInfo.IsTag(tag))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }
    }
}