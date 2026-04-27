using UnityEngine;

public class CowardAttackState : State<CowardStateEnum>
{
    private CowardEnemyController _enemy;

    public CowardAttackState(CowardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        _enemy.Attack();
    }
}