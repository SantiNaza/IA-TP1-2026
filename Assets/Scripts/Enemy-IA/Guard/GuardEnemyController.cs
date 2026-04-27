using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardEnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public LineOfSight los;
    public Transform[] waypoints;
    public SteeringAgent steeringAgent;
    public GuardDecisionTree decisionTree;

    private Transform _currentSteeringTarget;

    [Header("Movement")]
    public float waypointReachDistance = 0.5f;
    public float attackRange = 1.2f;

    [Header("Idle")]
    public float idleTime = 2f;

    private FSM<GuardStateEnum> _fsm;

    private int _currentWaypointIndex = 0;
    private int _direction = 1;

    private void Start()
    {
        InitializeFSM();

        if (decisionTree != null)
            decisionTree.enemy = this;
    }

    private void Update()
    {
        _fsm.OnUpdate();

        if (decisionTree == null) return;

        GuardStateEnum decision = decisionTree.Decide();

        // Interviene solo si hay jugador detectado
        if (decision == GuardStateEnum.Chase || decision == GuardStateEnum.Attack)
        {
            // evita re-transiciones innecesarias
            if (decision != _fsm.CurrentStateId)
                TransitionTo(decision);
        }
    }

    void InitializeFSM()
    {
        var patrol = new GuardPatrolState(this);
        var idle = new GuardIdleState(this);
        var chase = new GuardChaseState(this);
        var attack = new GuardAttackState(this);

        patrol.AddTransition(GuardStateEnum.Idle, idle);
        patrol.AddTransition(GuardStateEnum.Chase, chase);

        idle.AddTransition(GuardStateEnum.Patrol, patrol);
        idle.AddTransition(GuardStateEnum.Chase, chase);

        chase.AddTransition(GuardStateEnum.Patrol, patrol);
        chase.AddTransition(GuardStateEnum.Attack, attack);

        attack.AddTransition(GuardStateEnum.Patrol, patrol);

        _fsm = new FSM<GuardStateEnum>(patrol, GuardStateEnum.Patrol);
    }

    public void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform wp = waypoints[_currentWaypointIndex];

        Vector3 dir = wp.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < waypointReachDistance)
        {
            if (steeringAgent != null)
                steeringAgent.Stop();

            TransitionTo(GuardStateEnum.Idle);

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

            _currentSteeringTarget = null;
            return;
        }

        if (steeringAgent != null)
        {
            if (_currentSteeringTarget != wp)
            {
                _currentSteeringTarget = wp;
                steeringAgent.SetTarget(wp);
            }

            steeringAgent.MoveToTarget(true);
        }
    }

    public void ChaseTarget()
    {
        if (target == null || steeringAgent == null) return;

        steeringAgent.SetTarget(target);
        steeringAgent.MoveToTarget(false);
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

    public void TransitionTo(GuardStateEnum state)
    {
        _fsm.Transition(state);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}