using UnityEngine;

public class CowardRunAwayState : State<CowardStateEnum>
{
    private CowardEnemyController _enemy;

    public CowardRunAwayState(CowardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        _enemy.CacheSafeWaypoint();
    }

    public override void Execute()
    {
        if (!_enemy.CanSeeTarget())
        {
            if (_enemy.CanReachSafeDirectly())
            {
                _enemy.RunAwayDirect();
                return;
            }

            _enemy.TransitionTo(CowardStateEnum.Pathfinding);
            return;
        }
        _enemy.RunAway();
    }
}