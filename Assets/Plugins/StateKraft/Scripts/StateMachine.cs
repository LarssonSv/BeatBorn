using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StateKraft
{
    [Serializable]
    public class StateMachine
    {
        [SerializeField] private State[] _states;
        [SerializeField] private bool _firstStateIsDefault = true;

        public State CurrentState;
        private Dictionary<Type, State> _stateDictionary;
        private object _owner;

        public void Initialize(object owner)
        {
            _owner = owner;
            _stateDictionary = new Dictionary<Type, State>();
            //Create copies of all states
            foreach (State state in _states)
                _stateDictionary[state.GetType()] = Object.Instantiate(state);
            //Run init and set the statmachine variable of all created states
            foreach (State state in _stateDictionary.Values)
            {
                state.StateMachine = this;
                state.Initialize(owner);
            }
            //Transition to the first state in the list
            if (_firstStateIsDefault)
            {
                TransitionTo(_stateDictionary.First().Value);
                return;
            }
            //If the current state variable was set in the inspector, use that state type as the default state
            if (CurrentState != null)
                TransitionTo(GetState(CurrentState.GetType()));
        }
        public void Update()
        {
            if (CurrentState != null) CurrentState.StateUpdate();
        }

        public T GetState<T>()
        {
            return (T)Convert.ChangeType(_stateDictionary[typeof(T)], typeof(T));
        }
        public State GetState(Type type)
        {
            return _stateDictionary[type];
        }
        public T TransitionTo<T>()
        {
            T state = GetState<T>();
            TransitionTo(state as State);
            return state;

        }
        public State TransitionTo(State state)
        {
            if (state == null) { Debug.LogWarning("Cannot transition to state null"); return state; } //return needs to handle errors another way
            if (CurrentState != null) CurrentState.Exit();
            CurrentState = state;
            CurrentState.Enter();
            return state;
        }

        public void ReinitializeState(State state)
        {
            if (_owner == null) { Debug.LogWarning("Statemachine has not been initialized with valid owner"); return; }

            Type type = state.GetType();
            State instance = Object.Instantiate(state);
            instance.StateMachine = this;
            instance.Initialize(_owner);

            _stateDictionary[type] = instance;
            if (CurrentState.GetType() == type) TransitionTo(instance);
        }

        public void SetStates(State[] newStates)
        {
            _states = newStates;
        }
    }
}

