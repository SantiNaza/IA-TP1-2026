using UnityEngine;
using System.Collections.Generic;

public class State<T> : IState<T>
{
    Dictionary<T, IState<T>> _transitions = new Dictionary<T, IState<T>>();

    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }

    public void AddTransition(T input, IState<T> state)
    {
        _transitions[input] = state;
    }

    public IState<T> GetTransition(T input)
    {
        if (!_transitions.ContainsKey(input)) return null;
        return _transitions[input];
    }
}