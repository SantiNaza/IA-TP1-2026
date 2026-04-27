using UnityEngine;

public class CowardRunAwayState : State<CowardStateEnum>
{
    private CowardEnemyController _enemy;

    public CowardRunAwayState(CowardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        _enemy.RunAway();
    }
}