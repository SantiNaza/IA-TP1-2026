using UnityEngine;


public class CowardDecisionTree : MonoBehaviour
{
    public CowardEnemyController enemy;

    public CowardEnemyController.State Decide()
    {
        if (enemy == null)
            return CowardEnemyController.State.Patrol;

        // Nodo 1: ¿Ve al jugador?
        if (!enemy.CanSeeTarget())
            return CowardEnemyController.State.Patrol;

        // Nodo 2: ¿Jugador muy cerca?
        if (enemy.DistanceToTarget() <= enemy.trappedDistance)
        {
            // Nodo 3: ¿Está acorralado?
            if (enemy.IsBlocked())
                return CowardEnemyController.State.Attack;

            return CowardEnemyController.State.RunAway;
        }

        // Nodo 4: Si lo ve pero aún está lejos, huye.
        return CowardEnemyController.State.RunAway;
    }
}