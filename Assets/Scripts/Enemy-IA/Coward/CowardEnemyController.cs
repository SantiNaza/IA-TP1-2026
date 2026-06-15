using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CowardEnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public LineOfSight los;
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
 
    public Vector3 LastKnownPlayerPosition;

    [Header("Patrol Route")]
    public NavWaypoint[] patrolWaypoints;

    private List<NavWaypoint> currentPath = new List<NavWaypoint>();

    private int currentPathIndex;

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
        var pathfinding = new CowardPathfindingState(this);
        var safe = new CowardSafeState(this);
        var attack = new CowardAttackState(this);

        patrol.AddTransition(CowardStateEnum.Idle, idle);
        patrol.AddTransition(CowardStateEnum.RunAway, runAway);

        idle.AddTransition(CowardStateEnum.Patrol, patrol);

        runAway.AddTransition(CowardStateEnum.Patrol, patrol);
        runAway.AddTransition(CowardStateEnum.Attack, attack);
        runAway.AddTransition(CowardStateEnum.Pathfinding,pathfinding);

        attack.AddTransition(CowardStateEnum.Patrol, patrol);

        pathfinding.AddTransition(CowardStateEnum.Safe, safe);

        safe.AddTransition(CowardStateEnum.Patrol, patrol);
        safe.AddTransition(CowardStateEnum.RunAway, runAway);
        safe.AddTransition(CowardStateEnum.Attack, attack);
        safe.AddTransition(CowardStateEnum.Pathfinding, pathfinding);

        _fsm = new FSM<CowardStateEnum>(patrol, CowardStateEnum.Patrol);
    }


    public void Patrol()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;

        NavWaypoint wp = patrolWaypoints[_currentWaypointIndex];

        Vector3 dir = wp.transform.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < waypointReachDistance)
        {
            steeringAgent.Stop();
            TransitionTo(CowardStateEnum.Idle);

            Vector3 lookDir = wp.transform.position - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
                transform.forward = lookDir.normalized;

            _currentWaypointIndex += _direction;

            if (_currentWaypointIndex >= patrolWaypoints.Length)
            {
                _direction = -1;
                _currentWaypointIndex = patrolWaypoints.Length - 2;
            }
            else if (_currentWaypointIndex < 0)
            {
                _direction = 1;
                _currentWaypointIndex = 1;
            }

            _currentSteeringTarget = null;
            return;
        }

        if (_currentSteeringTarget != wp.transform)
        {
            _currentSteeringTarget = wp.transform;
            steeringAgent.SetTarget(wp.transform);
        }

        steeringAgent.MoveToTarget(true);
    }

    public void RunAway()
    {
        if (target == null) return;

        LastKnownPlayerPosition = target.position;

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

    public NavWaypoint GetClosestWaypoint(Vector3 position)
    {
        NavWaypoint closest = null;

        float closestDistance = Mathf.Infinity;

        foreach (NavWaypoint wp in NavigationManager.Instance.allWaypoints)
        {
            float distance =
                Vector3.Distance(position, wp.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = wp;
            }
        }

        return closest;
    }

    public NavWaypoint GetFarthestSafeWaypoint()
    {
        if (SafeWaypointManager.Instance == null)
            return null;

        NavWaypoint best = null;

        float farthestDistance = -1f;

        foreach (NavWaypoint wp in SafeWaypointManager.Instance.safeWaypoints)
        {
            float distance = Vector3.Distance(target.position, wp.transform.position);

            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                best = wp;
            }
        }

        return best;
    }

    public void CalculatePath()
    {
        NavWaypoint start = GetClosestWaypoint(transform.position);

        NavWaypoint safe = GetFarthestSafeWaypoint();

        if (safe == null)
        {
            currentPath.Clear();
            return;
        }

        currentPath = WaypointPathfinding.FindPath(start, safe);

        if (currentPath.Count == 0)
        {
            Debug.LogWarning("No path found!");
        }

        currentPathIndex = 0;
    }

    public void FollowPath()
    {
        if (currentPath == null)
            return;

        if (currentPathIndex >= currentPath.Count)
            return;

        NavWaypoint currentNode =
            currentPath[currentPathIndex];

        steeringAgent.SetTarget(currentNode.transform);
        steeringAgent.MoveToTarget(true);

        float distance =
            Vector3.Distance(transform.position, currentNode.transform.position);

        if (distance < waypointReachDistance)
        {
            currentPathIndex++;
        }
    }

    public bool PathFinished()
    {
        return currentPathIndex >= currentPath.Count;
    }
}