using UnityEngine;

public class GuardPathfindingState : State<GuardStateEnum>
{
    private GuardEnemyController enemy;

    public GuardPathfindingState(GuardEnemyController enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        enemy.CalculatePath();
    }

    public override void Execute()
    {
        if (enemy.CanSeeTarget())
        {
            enemy.TransitionTo(GuardStateEnum.Chase);
            return;
        }

        enemy.FollowPath();

        if (enemy.PathFinished())
        {
            enemy.TransitionTo(GuardStateEnum.Patrol);
        }
    }
}