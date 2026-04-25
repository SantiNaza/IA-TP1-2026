using UnityEngine;

public class EnemyAttackState : State<EnemyStateEnum>
{
    private EnemyController _enemy;

    public EnemyAttackState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        // Estado de ataque: si el enemigo entra en rango, se termina el juego.
        _enemy.GameOver();
    }
}
