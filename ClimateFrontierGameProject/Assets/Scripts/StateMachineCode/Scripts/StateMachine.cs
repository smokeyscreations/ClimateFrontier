using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState _currentState;

    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();

    private static readonly List<Transition> EmptyTransitions = new List<Transition>(0);

    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        _currentState?.Tick();
    }

    public void SetState(IState state)
    {
        if (_currentState == state)
            return;

        _currentState?.OnExit();
        //Debug.Log($"Transitioning to {state.GetType().Name}");
        _currentState = state;

        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        _currentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        var stateType = from.GetType();
        if (!_transitions.TryGetValue(stateType, out var transitions))
        {
            transitions = new List<Transition>();
            _transitions[stateType] = transitions;
        }

        transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(state, predicate));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
        {
            if (transition.Condition())
            {
                //Debug.Log("Any transition condition met.");
                return transition;
            }
        }

        foreach (var transition in _currentTransitions)
        {
            if (transition.Condition())
            {
                //Debug.Log("Current transition condition met.");
                return transition;
            }
        }

        return null;
    }
}
