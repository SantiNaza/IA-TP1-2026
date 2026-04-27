using UnityEngine;

public class CowardDecisionTree : MonoBehaviour
{
    public CowardEnemyController enemy;

    public CowardStateEnum Decide()
    {
        if (enemy == null)
            return CowardStateEnum.Patrol;

        // Nodo 1: Ve al jugador?
        if (!enemy.CanSeeTarget())
            return CowardStateEnum.Patrol;

        // Nodo 2: Jugador muy cerca?
        if (enemy.DistanceToTarget() <= enemy.trappedDistance)
        {
            // Nodo 3: Esta encerrado?
            if (enemy.IsBlocked())
                return CowardStateEnum.Attack;

            return CowardStateEnum.RunAway;
        }

        // Nodo 4: Si lo ve pero esta lejos, huye
        return CowardStateEnum.RunAway;
    }
}