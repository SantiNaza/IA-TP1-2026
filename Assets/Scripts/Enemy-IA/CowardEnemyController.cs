using UnityEngine;
using UnityEngine.SceneManagement;

public class CowardEnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public LineOfSight los;
    public Transform[] waypoints;
    public SteeringAgent steeringAgent;
    public CowardDecisionTree decisionTree;

    private Transform _currentSteeringTarget;

    [Header("Movement")]
    public float waypointReachDistance = 0.5f;
    public float attackRange = 1.5f;
    public float trappedDistance = 2f;

    [Header("Idle")]
    public float idleTime = 2f;

    private int _currentWaypointIndex = 0;
    private int _direction = 1;
    private float idleTimer;

    public enum State
    {
        Patrol,
        Idle,
        RunAway,
        Attack
    }

    public State currentState;

    void Start()
    {
        currentState = State.Patrol;

        if (decisionTree != null)
            decisionTree.enemy = this;
    }

    void Update()
    {
        // Si no está idle, decide constantemente
        if (currentState != State.Idle && decisionTree != null)
        {
            currentState = decisionTree.Decide();
        }

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
    }

    // ==================================================
    // PATROL
    // ==================================================
    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform wp = waypoints[_currentWaypointIndex];

        Vector3 dir = wp.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < waypointReachDistance)
        {
            steeringAgent.Stop();

            currentState = State.Idle;
            idleTimer = idleTime;

            Vector3 lookDir = wp.position - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
                transform.forward = lookDir.normalized;

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

            return;
        }

        if (_currentSteeringTarget != wp)
        {
            _currentSteeringTarget = wp;
            steeringAgent.SetTarget(wp);
        }

        steeringAgent.MoveToTarget(true);
    }

    // ==================================================
    // IDLE
    // ==================================================
    void Idle()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
            currentState = State.Patrol;
    }

    // ==================================================
    // RUN AWAY
    // ==================================================
    void RunAway()
    {
        if (target == null) return;

        steeringAgent.SetTarget(target);
        steeringAgent.MoveAwayFromTarget();
    }

    // ==================================================
    // ATTACK
    // ==================================================
    void Attack()
    {
        if (target == null) return;

        steeringAgent.SetTarget(target);
        steeringAgent.MoveToTarget(false);

        if (DistanceToTarget() <= attackRange)
        {
            GameOver();
        }
    }

    // ==================================================
    // MÉTODOS USADOS POR DECISION TREE
    // ==================================================
    public bool CanSeeTarget()
    {
        if (los == null || target == null) return false;
        return los.CanSeeTarget(target);
    }

    public float DistanceToTarget()
    {
        if (target == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, target.position);
    }

    public bool IsBlocked()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
            transform.forward, out hit, 1.5f))
        {
            if (!hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    // ==================================================
    // GAME OVER
    // ==================================================
    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}