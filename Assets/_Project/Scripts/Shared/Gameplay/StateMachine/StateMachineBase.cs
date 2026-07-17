using UnityEngine;

namespace GameplaySystemsAndTools.Shared.Gameplay.StateMachine
{
    /// <summary>
    /// MonoBehaviour host of a hierarchical FSM: holds the current root state and
    /// ticks the whole state chain (root -> leaf) every frame.
    /// </summary>
    public abstract class StateMachineBase : MonoBehaviour
    {
        private StateBase currentState;
        public StateBase CurrentState => currentState;

        private StateBase previousState;
        public StateBase PreviousState => previousState;
        public StateBase PreviousLeafState { get; internal set; }

        void Update()
        {
            currentState?.UpdateStates(Time.deltaTime);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SwitchState(StateBase newState)
        {
            currentState?.Exit();
            previousState = currentState;
            currentState = newState;
            currentState?.Enter();
        }
    }
}