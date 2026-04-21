using UnityEngine;

public class EnemyPatrolState : State<EnemyStateEnum>
{
    private EnemyController _enemy;

    public EnemyPatrolState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        //Patrulla waypoints hasta que ve al jugador.
        if (_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(EnemyStateEnum.Chase);
            return;
        }

        _enemy.Patrol();
    }
}