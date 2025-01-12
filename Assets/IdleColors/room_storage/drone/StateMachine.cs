using IdleColors.room_storage.drone.states;
using UnityEngine;

namespace IdleColors.room_storage.drone
{
    // die update in die erbende klasse ... 
    public abstract class StateMachine : MonoBehaviour
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