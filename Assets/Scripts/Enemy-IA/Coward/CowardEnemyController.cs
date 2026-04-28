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

    private FSM<CowardStateEnum> _fsm;

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

        if (_fsm.CurrentStateId == CowardStateEnum.Idle) return;
        if (decisionTree == null) return;

        CowardStateEnum decision = decisionTree.Decide();

        if (decision != _fsm.CurrentStateId)
            _fsm.Transition(decision);

    }

    private void InitializeFSM()
    {
        var patrol = new CowardPatrolState(this);
        var idle = new CowardIdleState(this);
        var runAway = new CowardRunAwayState(this);
        var attack = new CowardAttackState(this);

        patrol.AddTransition(CowardStateEnum.Idle, idle);
        patrol.AddTransition(CowardStateEnum.RunAway, runAway);

        idle.AddTransition(CowardStateEnum.Patrol, patrol);

        runAway.AddTransition(CowardStateEnum.Patrol, patrol);
        runAway.AddTransition(CowardStateEnum.Attack, attack);

        attack.AddTransition(CowardStateEnum.Patrol, patrol);

        _fsm = new FSM<CowardStateEnum>(patrol, CowardStateEnum.Patrol);
    }


    public void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform wp = waypoints[_currentWaypointIndex];

        Vector3 dir = wp.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < waypointReachDistance)
        {
            steeringAgent.Stop();
            TransitionTo(CowardStateEnum.Idle);

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

        if (_currentSteeringTarget != wp)
        {
            _currentSteeringTarget = wp;
            steeringAgent.SetTarget(wp);
        }

        steeringAgent.MoveToTarget(true);
    }

    public void RunAway()
    {
        if (target == null) return;

        steeringAgent.SetTarget(target);
        steeringAgent.MoveAwayFromTarget();
    }

    public void Attack()
    {
        if (target == null) return;

        steeringAgent.SetTarget(target);
        steeringAgent.MoveToTarget(false);

        if (DistanceToTarget() <= attackRange)
            GameOver();
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

    public void TransitionTo(CowardStateEnum state)
    {
        _fsm.Transition(state);
    }

    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public string GetCurrentState()
    {
        return _fsm.CurrentStateId.ToString();
    }
}