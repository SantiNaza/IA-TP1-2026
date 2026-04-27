using UnityEngine;

public class GuardAttackState : State<GuardStateEnum>
{
    private GuardEnemyController _enemy;

    public GuardAttackState(GuardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        // Estado de ataque: si el enemigo entra en rango, se termina el juego.
        _enemy.GameOver();
    }
}
