using UnityEngine;

public class CowardSafeState : State<CowardStateEnum>
{
    private CowardEnemyController enemy;
    private float timer;

    public CowardSafeState(CowardEnemyController enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        timer = 0f;
        enemy.steeringAgent.Stop();
    }

    public override void Execute()
    {
        if (enemy.CanSeeTarget())
        {
            enemy.TransitionTo(CowardStateEnum.RunAway);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            enemy.TransitionTo(CowardStateEnum.Patrol);
        }
    }
}