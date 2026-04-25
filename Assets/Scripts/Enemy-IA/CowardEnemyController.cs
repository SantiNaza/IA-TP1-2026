using UnityEngine;
using UnityEngine.SceneManagement;

public class CowardEnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public LineOfSight los;
    public Transform[] waypoints;

    [Header("Movement")]
    public float speed = 3f;
    public float waypointReachDistance = 0.5f;
    public float attackRange = 1.5f;

    [Header("Run Away")]
    public float trappedDistance = 2f;
    public float obstacleCheckDistance = 1.5f;

    [Header("Idle")]
    public float idleTime = 2f;

    private int _currentWaypointIndex = 0;
    private int _direction = 1;

    private enum State
    {
        Patrol,
        Idle,
        RunAway,
        Attack
    }

    private State currentState;
    private float idleTimer;

    void Start()
    {
        currentState = State.Patrol;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Idle:
                Idle();
                break;

            case State.RunAway:
                RunAway();
                break;

            case State.Attack:
                Attack();
                break;
        }

        DetectPlayer();
    }

    // ===================================================
    // DETECCION
    // ===================================================
    void DetectPlayer()
    {
        if (target == null || los == null) return;

        if (los.CanSeeTarget(target))
        {
            if (currentState == State.Patrol || currentState == State.Idle)
                currentState = State.RunAway;
        }
    }

    // ===================================================
    // PATROL
    // ===================================================
    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform wp = waypoints[_currentWaypointIndex];

        float dist = Vector3.Distance(transform.position, wp.position);

        if (dist <= waypointReachDistance)
        {
            _currentWaypointIndex += _direction;

            if (_currentWaypointIndex >= waypoints.Length)
            {
                _direction = -1;
                _currentWaypointIndex = waypoints.Length - 2;
            }
            else if (_currentWaypointIndex < 0)
            {
                _direction = 1;
                _currentWaypointIndex = 1;
            }

            currentState = State.Idle;
            idleTimer = idleTime;
            return;
        }

        Vector3 dir = SteeringBehaviours.Seek(transform, wp.position, speed);
        dir += SteeringBehaviours.ObstacleAvoidance(transform);

        Move(dir);
    }

    // ===================================================
    // IDLE
    // ===================================================
    void Idle()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
            currentState = State.Patrol;
    }

    // ===================================================
    // RUN AWAY
    // ===================================================
    void RunAway()
    {
        Vector3 dir = SteeringBehaviours.Evade(transform, target, speed);
        dir += SteeringBehaviours.ObstacleAvoidance(transform);

        Move(dir);

        float dist = Vector3.Distance(transform.position, target.position);

        // Si no puede escapar o jugador muy cerca => atacar
        if (dist < trappedDistance || IsBlocked())
        {
            currentState = State.Attack;
        }
    }

    // ===================================================
    // ATTACK
    // ===================================================
    void Attack()
    {
        Vector3 dir = SteeringBehaviours.Pursuit(transform, target, speed);
        dir += SteeringBehaviours.ObstacleAvoidance(transform);

        Move(dir);

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackRange)
        {
            GameOver();
        }

        if (!los.CanSeeTarget(target))
        {
            currentState = State.Patrol;
        }
    }

    // ===================================================
    // MOVIMIENTO
    // ===================================================
    void Move(Vector3 dir)
    {
        dir.y = 0;
        dir = dir.normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.forward = dir;
    }

    // ===================================================
    // BLOQUEADO
    // ===================================================
    bool IsBlocked()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleCheckDistance))
        {
            if (!hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    // ===================================================
    // GAME OVER
    // ===================================================
    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}