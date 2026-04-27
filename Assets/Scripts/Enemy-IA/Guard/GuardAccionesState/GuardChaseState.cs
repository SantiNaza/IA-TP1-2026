using UnityEngine;

public class GuardChaseState : State<GuardStateEnum>
{
    private GuardEnemyController _enemy;

    public GuardChaseState(GuardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        // Si pierde al jugador, vuelve a patrullar.
        if (!_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(GuardStateEnum.Patrol);
            return;
        }

        // Si el jugador esta cerca, lo agarro y perdio.
        if (_enemy.DistanceToTarget() <= _enemy.attackRange)
        {
            _enemy.TransitionTo(GuardStateEnum.Attack);
            return;
        }

        _enemy.ChaseTarget();
    }
}