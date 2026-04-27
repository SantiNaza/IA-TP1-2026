using UnityEngine;

public class GuardDecisionTree : MonoBehaviour
{
    public GuardEnemyController enemy;

    public GuardStateEnum Decide()
    {
        if (enemy == null)
            return GuardStateEnum.Patrol;

        // Nodo 1: Ve al jugador?
        if (!enemy.CanSeeTarget())
            return GuardStateEnum.Patrol;

        // Nodo 2: Esta cerca del jugador para atacar?
        if (enemy.DistanceToTarget() <= enemy.attackRange)
            return GuardStateEnum.Attack;

        // Nodo 3: Si lo ve pero esta lejos, lo persigue
        return GuardStateEnum.Chase;
    }
}