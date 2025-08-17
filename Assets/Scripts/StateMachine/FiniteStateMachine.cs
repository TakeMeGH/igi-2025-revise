using UnityEngine;

namespace Perspective.StateMachine
{
    public abstract class FiniteStateMachine : MonoBehaviour
    {
        protected IState CurrentState;
        protected bool OnSwitchState;

        public void SwitchState(IState newState)
        {
            OnSwitchState = true;
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
            OnSwitchState = false;
        }

        private void Update()
        {
            if (!OnSwitchState) CurrentState?.Update();
        }

        private void FixedUpdate()
        {
            if (!OnSwitchState) CurrentState?.PhysicsUpdate();
        }

        public System.Type GetCurrentStateType()
        {
            return CurrentState.GetType();
        }

        public IState GetCurrentState()
        {
            return CurrentState;
        }
    }
}