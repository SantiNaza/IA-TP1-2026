using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardEnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public LineOfSight los;
    public SteeringAgent steeringAgent;
    public GuardDecisionTree decisionTree;

    private Transform _currentSteeringTarget;

    [Header("Movement")]
    public float speed = 2.5f;
    public float waypointReachDistance = 1.2f;
    public float attackRange = 1.2f;

    [Header("Idle")]
    public float idleTime = 2f;

    private FSM<GuardStateEnum> _fsm;

    private int _currentWaypointIndex = 0;
    private int _direction = 1;

    private bool skipNextRoulette = false;
    private bool isWaiting = false;

    public Vector3 LastKnownPlayerPosition;

    [Header("Patrol Route")]
    public NavWaypoint[] patrolWaypoints;

    private List<NavWaypoint> currentPath = new List<NavWaypoint>();

    private int currentPathIndex;

    private enum GuardPatrolAction
    {
        WAIT,
        BACK,
        FORWARD_TWO
    }

    private RouletteWheelSelector<GuardPatrolAction> _rouletteSelector;

    private void Start()
    {
        InitializeFSM();

        if (decisionTree != null)
            decisionTree.enemy = this;

        // Inicializar y registrar opciones con pesos por defecto (estado normal)
        _rouletteSelector = new RouletteWheelSelector<GuardPatrolAction>();
        _rouletteSelector.Register(GuardPatrolAction.WAIT, 50f);
        _rouletteSelector.Register(GuardPatrolAction.BACK, 30f);
        _rouletteSelector.Register(GuardPatrolAction.FORWARD_TWO, 20f);
    }

    private void Update()
    {
        _fsm.OnUpdate();

        if (decisionTree == null) return;

        GuardStateEnum decision = decisionTree.Decide();

        if (decision == GuardStateEnum.Chase || decision == GuardStateEnum.Attack)
        {
            if (decision != _fsm.CurrentStateId)
                TransitionTo(decision);
        }

    }

    void InitializeFSM()
    {
        var patrol = new GuardPatrolState(this);
        var idle = new GuardIdleState(this);
        var chase = new GuardChaseState(this);
        var pathfinding = new GuardPathfindingState(this);
        var attack = new GuardAttackState(this);

        patrol.AddTransition(GuardStateEnum.Idle, idle);
        patrol.AddTransition(GuardStateEnum.Chase, chase);

        idle.AddTransition(GuardStateEnum.Patrol, patrol);
        idle.AddTransition(GuardStateEnum.Chase, chase);

        chase.AddTransition(GuardStateEnum.Pathfinding, pathfinding);
        chase.AddTransition(GuardStateEnum.Attack, attack);

        pathfinding.AddTransition(GuardStateEnum.Chase, chase);
        pathfinding.AddTransition(GuardStateEnum.Patrol, patrol);

        attack.AddTransition(GuardStateEnum.Patrol, patrol);

        _fsm = new FSM<GuardStateEnum>(patrol, GuardStateEnum.Patrol);
    }

    public void Patrol()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;
        if (isWaiting) return;

        NavWaypoint wp = patrolWaypoints[_currentWaypointIndex];

        Vector3 dir = wp.transform.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < waypointReachDistance)
        {
            if (steeringAgent != null)
                steeringAgent.Stop();

            Vector3 lookDir = wp.transform.position - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
                transform.forward = lookDir.normalized;

            if (skipNextRoulette)
            {
                skipNextRoulette = false;
                _currentWaypointIndex += _direction;
                ClampWaypointIndex();
                _currentSteeringTarget = null;
                return;
            }

            if (_currentWaypointIndex < patrolWaypoints.Length / 2)
            {
                _rouletteSelector.UpdateWeight(GuardPatrolAction.WAIT, 20f);
                _rouletteSelector.UpdateWeight(GuardPatrolAction.BACK, 30f);
                _rouletteSelector.UpdateWeight(GuardPatrolAction.FORWARD_TWO, 50f);
            }
            else
            {

                _rouletteSelector.UpdateWeight(GuardPatrolAction.WAIT, 50f);
                _rouletteSelector.UpdateWeight(GuardPatrolAction.BACK, 30f);
                _rouletteSelector.UpdateWeight(GuardPatrolAction.FORWARD_TWO, 20f);
            }

            GuardPatrolAction action = _rouletteSelector.Select();

            switch (action)
            {
                case GuardPatrolAction.WAIT: // WAIT
                    StartCoroutine(WaitAndContinue(2f));
                    break;

                case GuardPatrolAction.BACK: // BACK 1
                    _currentWaypointIndex -= _direction;
                    ClampWaypointIndex();
                    break;

                case GuardPatrolAction.FORWARD_TWO: // FORWARD 2
                    _currentWaypointIndex += _direction * 2;
                    ClampWaypointIndex();
                    skipNextRoulette = true;
                    break;
            }

            _currentSteeringTarget = null;
            return;
        }

        if (steeringAgent != null)
        {
            if (_currentSteeringTarget != wp.transform)
            {
                _currentSteeringTarget = wp.transform;
                steeringAgent.SetTarget(wp.transform);
            }

            steeringAgent.MoveToTarget(true);
        }
    }

    IEnumerator WaitAndContinue(float time)
    {
        isWaiting = true;

        if (steeringAgent != null)
            steeringAgent.Stop();

        yield return new WaitForSeconds(time);

        _currentWaypointIndex += _direction;
        ClampWaypointIndex();
        _currentSteeringTarget = null;

        isWaiting = false;
    }

    void ClampWaypointIndex()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;

        if (_currentWaypointIndex >= patrolWaypoints.Length)
        {
            _currentWaypointIndex = patrolWaypoints.Length - 1;
            _direction = -1;
        }
        else if (_currentWaypointIndex < 0)
        {
            _currentWaypointIndex = 0;
            _direction = 1;
        }
    }

    public void ChaseTarget()
    {
        if (target == null || steeringAgent == null) return;

        LastKnownPlayerPosition = target.position;

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

    public void CalculatePath()
    {
        NavWaypoint start =
            GetClosestWaypoint(transform.position);

        NavWaypoint end =
            GetClosestWaypoint(LastKnownPlayerPosition);

        currentPath =
            WaypointPathfinding.FindPath(start, end);

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
            Vector3.Distance(
                transform.position,
                currentNode.transform.position);

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