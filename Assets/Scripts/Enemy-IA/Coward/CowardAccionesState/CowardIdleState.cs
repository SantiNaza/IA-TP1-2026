using UnityEngine;

public class CowardIdleState : State<CowardStateEnum>
{
    private CowardEnemyController _enemy;
    private float _timer;

    public CowardIdleState(CowardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        _timer = 0f;
        _enemy.steeringAgent.Stop();
        _enemy.TransitionTo(CowardStateEnum.Patrol);
    }

    public override void Execute()
    {
        _timer += Time.deltaTime;

        if (_timer >= _enemy.idleTime)
        {
            _enemy.TransitionTo(CowardStateEnum.Patrol);
        }
    }
}