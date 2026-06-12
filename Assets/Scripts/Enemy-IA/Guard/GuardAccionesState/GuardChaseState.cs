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
        // pathfinding si no veo al jugador, sino lo persigo. Si lo pierdo, busco por el ultimo lugar donde lo vi.
        if (!_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(
                GuardStateEnum.Pathfinding);

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