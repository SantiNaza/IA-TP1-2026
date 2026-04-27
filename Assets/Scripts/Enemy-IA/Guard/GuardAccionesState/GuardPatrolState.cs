using UnityEngine;

public class GuardPatrolState : State<GuardStateEnum>
{
    private GuardEnemyController _enemy;

    public GuardPatrolState(GuardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        //Patrulla waypoints hasta que ve al jugador.
        if (_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(GuardStateEnum.Chase);
            return;
        }

        _enemy.Patrol();
    }
}