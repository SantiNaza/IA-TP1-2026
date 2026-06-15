using UnityEngine;

public class CowardPathfindingState: State<CowardStateEnum>
{
    private CowardEnemyController enemy;

    public CowardPathfindingState(CowardEnemyController enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        enemy.CalculatePath();
    }

    public override void Execute()
    {
        enemy.FollowPath();

        if (enemy.PathFinished())
        {
            enemy.TransitionTo(CowardStateEnum.Safe);
        }
    }
}
