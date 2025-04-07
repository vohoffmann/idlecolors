using IdleColors.room_order.constructor.states;
using UnityEngine;

namespace IdleColors.room_order.constructor
{
    public abstract class StateManager : MonoBehaviour
    {
        protected State _currentState;

        public void ChangeState(State newState)
        {
            _currentState?.Exit();

            _currentState = newState;
            _currentState.Enter();
        }
    }
}