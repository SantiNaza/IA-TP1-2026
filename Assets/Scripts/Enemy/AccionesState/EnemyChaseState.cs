using UnityEngine;

public class EnemyChaseState : State<EnemyStateEnum>
{
    private EnemyController _enemy;

    public EnemyChaseState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        // Si pierde al jugador, vuelve a patrullar.
        if (!_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(EnemyStateEnum.Patrol);
            return;
        }

        // Si el jugador esta cerca, lo agarro y perdio.
        if (_enemy.DistanceToTarget() <= _enemy.attackRange)
        {
            _enemy.TransitionTo(EnemyStateEnum.Attack);
            return;
        }

        _enemy.ChaseTarget();
    }
}