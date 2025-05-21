using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private readonly Dictionary<Type, State> _states;
    private State _currentState;
    public State PreviousState { get; private set; }

    public StateMachine(params State[] states)
    {
        _states = new Dictionary<Type, State>();
        foreach (var s in states)
        {
            _states.Add(s.GetType(), s);
        }

        TransitionToState(states[0]);
    }
    
    public void TransitionToState<T>() where T : State
    {
        var newState = GetState<T>();
        if (newState == null)
        {
            Debug.LogError(typeof(T).Name + " is not a valid state");
            return;
        }
        TransitionToState(newState);
    }
    
    public void TransitionToState(State newState)
    {
        if(newState == _currentState) return;
            
        PreviousState = _currentState;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
        
    public T GetState<T>() where T : State
    {
        return _states[typeof(T)] as T;
    }

    public void Update()
    {
        _currentState.Update();
    }
    
    public void FixedUpdate()
    {
        _currentState.FixedUpdate();
    }
    
    public void LateUpdate()
    {
        _currentState.LateUpdate();
    }

    public State ActiveState => _currentState;
}