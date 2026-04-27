using UnityEngine;

public interface IState<T>
{
    void Enter();
    void Execute();
    void Exit();
    void AddTransition(T input, IState<T> state);
    IState<T> GetTransition(T input);
}

