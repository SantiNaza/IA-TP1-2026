using UnityEngine;

public class CowardPatrolState : State<CowardStateEnum>
{
    private CowardEnemyController _enemy;

    public CowardPatrolState(CowardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        _enemy.Patrol();
    }
}