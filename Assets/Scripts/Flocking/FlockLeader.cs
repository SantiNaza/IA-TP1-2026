using UnityEngine;
using UnityEngine.SceneManagement;

public class FlockLeader : MonoBehaviour
{
    [Header("FSM & Vision")]
    public GuardStateEnum currentState = GuardStateEnum.Patrol;
    public Transform targetPlayer;
    public LineOfSight los;
    public float attackRange = 1.2f;

    [Header("Waypoints")]
    public Transform[] waypoints;
    public float speed = 3f;
    public float rotationSpeed = 5f;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleMask;
    public float avoidanceRadius = 4f;
    public float avoidanceWeight = 15f;

    private int _currentWaypointIndex;

    void Update()
    {
        CheckStateTransitions();

        Vector3 desiredDirection = Vector3.zero;

        // 1. Ejecutar el comportamiento según el estado
        switch (currentState)
        {
            case GuardStateEnum.Patrol:
                desiredDirection = GetPatrolDirection();
                break;
            case GuardStateEnum.Chase:
                desiredDirection = GetChaseDirection();
                break;
            case GuardStateEnum.Attack:
                AttackPlayer();
                return; // Cortamos aquí para que no intente moverse más
        }

        // 2. Obstacle Avoidance (Siempre activo, incluso si persigue)
        Vector3 avoidanceForce = CalculateObstacleAvoidance();
        Vector3 finalDirection = desiredDirection + (avoidanceForce * avoidanceWeight);
        finalDirection.y = 0;

        // 3. Mover al líder
        if (finalDirection.magnitude > 0.01f)
        {
            Vector3 velocity = finalDirection.normalized;
            transform.position += velocity * speed * Time.deltaTime;

            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    // ==========================================
    // LOGICA DE ESTADOS
    // ==========================================
    void CheckStateTransitions()
    {
        if (targetPlayer == null) return;
        if (currentState == GuardStateEnum.Attack) return;

        float distToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        // Transición a ATAQUE
        if (distToPlayer <= attackRange)
        {
            currentState = GuardStateEnum.Attack;
        }
        // Transición a CHASE (Usando tu script LineOfSight)
        else if (los != null && los.CanSeeTarget(targetPlayer))
        {
            currentState = GuardStateEnum.Chase;
        }
        // Opcional: Podrías añadir lógica aquí para volver a Patrol si lo pierde de vista
    }

    Vector3 GetPatrolDirection()
    {
        if (waypoints == null || waypoints.Length == 0) return transform.forward;

        Transform wp = waypoints[_currentWaypointIndex];
        Vector3 dir = (wp.position - transform.position).normalized;

        if (Vector3.Distance(transform.position, wp.position) < 0.5f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        }

        return dir;
    }

    Vector3 GetChaseDirection()
    {
        if (targetPlayer == null) return transform.forward;
        
        // Va directo hacia el jugador
        return (targetPlayer.position - transform.position).normalized;
    }

    void AttackPlayer()
    {
        // Misma lógica de Game Over que tienes en tu GuardEnemyController
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ==========================================
    // EVASIÓN DE OBSTÁCULOS
    // ==========================================
    Vector3 CalculateObstacleAvoidance()
    {
        Vector3 avoidForce = Vector3.zero;
        RaycastHit hit;

        Vector3[] rayDirections = {
            transform.forward,
            Quaternion.Euler(0, -35, 0) * transform.forward,
            Quaternion.Euler(0, 35, 0) * transform.forward
        };

        foreach (Vector3 dir in rayDirections)
        {
            if (Physics.Raycast(transform.position, dir, out hit, avoidanceRadius, obstacleMask))
            {
                float distanceRatio = 1f - (hit.distance / avoidanceRadius);
                avoidForce += hit.normal * distanceRatio;
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }

        return avoidForce.normalized;
    }
}