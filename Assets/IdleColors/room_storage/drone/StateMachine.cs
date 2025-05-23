﻿using IdleColors.room_storage.drone.states;
using UnityEngine;

namespace IdleColors.room_storage.drone
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State _currentState;

        public void ChangeState(State newState)
        {
            _currentState = newState;
            _currentState.Enter();
        }
    }
}