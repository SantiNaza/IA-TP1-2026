using UnityEngine;

public class FSM<T>
{
    // FSM generica: maneja estados y transiciones.
    // Cada estado define que hacer en execute y que pasa al entrar/salir.
    IState<T> _current;
    public T CurrentStateId { get; private set; }

    public FSM(IState<T> init, T initId)
    {
        SetInitial(init, initId);
    }

    public void SetInitial(IState<T> init, T initId)
    {
        _current = init;
        CurrentStateId = initId;
        _current.Enter();
    }

    public void OnUpdate()
    {
        _current?.Execute();
    }

    public void Transition(T input)
    {
        IState<T> newState = _current.GetTransition(input);
        if (newState == null) return;

        _current.Exit();
        _current = newState;
        CurrentStateId = input;
        _current.Enter();
    }
}