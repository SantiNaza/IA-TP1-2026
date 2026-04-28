using System.Collections;
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

    private int _lastRouletteResult = -1;
    private bool skipNextRoulette = false;
    private bool isWaiting = false;

    private void Start()
    {
        InitializeFSM();

        if (decisionTree != null)
            decisionTree.enemy = this;
    }

    private void Update()
    {
        // Cuando isWaiting es verdadero, detenemos la FSM y cualquier decisión
        // Esto asegura que el enemigo permanezca quieto durante unos segundos
        if (!isWaiting)
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

    //ruleta 
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

            Vector3 lookDir = wp.position - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
                transform.forward = lookDir.normalized;

            // Si el resultado anterior fue ForwardTwo, saltamos la ruleta una vez y avanzamos solo 1 waypoint normal
            if (skipNextRoulette)
            {
                skipNextRoulette = false;
                _currentWaypointIndex += _direction;
                ClampWaypointIndex();
                _currentSteeringTarget = null;
                return;
            }

            Debug.Log($"GuardEnemyController: entered waypoint, rolling roulette at index {_currentWaypointIndex}", this);
            int result = RollRoulette();

            switch (result)
            {
                case 0:
                    Debug.Log("Roulette result 0: WAIT for 5 seconds.", this);
                    TransitionTo(GuardStateEnum.Idle);
                    StartCoroutine(WaitAndContinue(5f));
                    break;

                case 1:
                    Debug.Log("Roulette result 1: move back one waypoint.", this);
                    _direction = -_direction;
                    _currentWaypointIndex += _direction;
                    ClampWaypointIndex();
                    break;

                case 2:
                    Debug.Log("Roulette result 2: move forward exactly two waypoints.", this);
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
            if (_currentSteeringTarget != wp)
            {
                _currentSteeringTarget = wp;
                steeringAgent.SetTarget(wp);
            }

            steeringAgent.MoveToTarget(true);
        }
    }

    // Coroutine: espera el tiempo, avanza 1 waypoint normal y reanuda la FSM
    IEnumerator WaitAndContinue(float time)
    {
        // Marca que estamos en el modo de espera y evita que Update siga ejecutando estados.
        Debug.Log("WaitAndContinue: waiting started.", this);
        isWaiting = true;

        if (steeringAgent != null)
            steeringAgent.Stop();

        yield return new WaitForSeconds(time);

        Debug.Log("WaitAndContinue: waiting finished.", this);

        // Avanza un waypoint normal al terminar la espera
        _currentWaypointIndex += _direction;
        ClampWaypointIndex();
        _currentSteeringTarget = null;

        isWaiting = false;
        TransitionTo(GuardStateEnum.Patrol);
    }

    // Retorna 0, 1 o 2, evitando dos WAIT seguidos para dar más variación.
    int RollRoulette()
    {
        int result = Random.Range(0, 3);

        if (_lastRouletteResult == 0 && result == 0)
        {
            // Si el resultado anterior fue WAIT, forzamos un 1 o 2 para no repetirlo.
            result = Random.Range(1, 3);
            Debug.Log("RollRoulette: repeated WAIT avoided, choosing 1 or 2.", this);
        }

        _lastRouletteResult = result;
        return result;
    }

    // Evita que el índice salga del array y ajusta la dirección en los extremos.
    void ClampWaypointIndex()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (_currentWaypointIndex >= waypoints.Length)
        {
            _currentWaypointIndex = waypoints.Length - 1;
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