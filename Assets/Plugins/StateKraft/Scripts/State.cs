using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StateKraft
{
    public abstract class State : ScriptableObject
    {
        [HideInInspector] public StateMachine StateMachine;
        public Color StateGizmoColor = Color.green;  // Simon Added
        public virtual void Initialize(object owner) { }
        public virtual void Enter() { }
        public virtual void StateUpdate() { }
        public virtual void Exit() { }

        public void TransitionTo()
        {
            StateMachine.TransitionTo(this);
        }
        public void TransitionTo(State state)
        {
            StateMachine.TransitionTo(state);
        }

        public T TransitionTo<T>()
        {
            return StateMachine.TransitionTo<T>();
        }

        public T GetState<T>()
        {
            return StateMachine.GetState<T>();
        }
    }
}

