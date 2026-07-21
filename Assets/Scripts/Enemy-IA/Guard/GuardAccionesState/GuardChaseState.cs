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
        if (_enemy.DistanceToTarget() <= _enemy.attackRange)
        {
            _enemy.TransitionTo(GuardStateEnum.Attack);
            return;
        }

        // Si pierde de vista al jugador va a Alert
        if (!_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(GuardStateEnum.Alert);
            return;
        }

        _enemy.ChaseTarget();
    }
}