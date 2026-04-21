using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public LineOfSight los;
    public Transform[] waypoints;

    [Header("Movement")]
    public float speed = 3f;
    public float waypointReachDistance = 0.5f;
    public float attackRange = 1.2f;

    [Header("Idle")]
    public float idleTime = 2f;

    private FSM<EnemyStateEnum> _fsm;

    private int _currentWaypointIndex = 0;
    private int _direction = 1; // 1 = va hacia adelante, -1 = vuelve

    private void Start()
    {
        InitializeFSM();
    }

    private void Update()
    {
        _fsm.OnUpdate();
    }

    void InitializeFSM()
    {
        var patrol = new EnemyPatrolState(this);
        var idle = new EnemyIdleState(this);
        var chase = new EnemyChaseState(this);
        var attack = new EnemyAttackState(this);

        patrol.AddTransition(EnemyStateEnum.Idle, idle);
        patrol.AddTransition(EnemyStateEnum.Chase, chase);

        idle.AddTransition(EnemyStateEnum.Patrol, patrol);
        idle.AddTransition(EnemyStateEnum.Chase, chase);

        chase.AddTransition(EnemyStateEnum.Patrol, patrol);
        chase.AddTransition(EnemyStateEnum.Attack, attack);

        attack.AddTransition(EnemyStateEnum.Patrol, patrol);

        _fsm = new FSM<EnemyStateEnum>(patrol);
    }

    // Patrulla recorriendo waypoints ida y vuelta.
    public void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform wp = waypoints[_currentWaypointIndex];

        Vector3 dir = wp.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < waypointReachDistance)
        {
            // Cada vez que llega a un waypoint, pasa a Idle
            TransitionTo(EnemyStateEnum.Idle);

            // Cambia el índice para el próximo waypoint (ida y vuelta)
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

        Move(dir);
    }

    public void ChaseTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        Move(dir);
    }

    public void Move(Vector3 dir)
    {
        dir = dir.normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.forward = dir;
    }

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

    public void TransitionTo(EnemyStateEnum state)
    {
        _fsm.Transition(state);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}