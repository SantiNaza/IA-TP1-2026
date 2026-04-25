using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 3f;
    public float slowingDistance = 2f;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleMask;
    public float avoidanceRadius = 2f;
    public float avoidanceAngle = 70f;

    private Rigidbody _rb;
    private Vector3 _velocity;

    private Transform _target;

    private Arrive _arrive;
    private Seek _seek;
    private ObstacleAvoidance _avoidance;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        //Inicia el sistema de avoidance para detectar obstaculos cerca del NPC.
        _avoidance = new ObstacleAvoidance(transform, avoidanceRadius, avoidanceAngle, obstacleMask);
    }

    public void SetTarget(Transform target)
    {
        _target = target;

        if (_target == null) return;

        _seek = new Seek(_target, transform, maxSpeed);
        _arrive = new Arrive(_target, transform, maxSpeed, slowingDistance);
    }

    public void Stop()
    {
        _velocity = Vector3.zero;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void MoveToTarget(bool useArrive)
    {
        if (_target == null) return;

        // Elegimos el steering segun el estado (Patrol = Arrive, Chase = Seek)
        Vector3 desiredVelocity = useArrive ? _arrive.GetDir(_velocity) : _seek.GetDir(_velocity);

        if (desiredVelocity.magnitude < 0.05f)
        {

            _velocity = transform.forward * 0.1f;
            desiredVelocity = _velocity;
        }

        // Si hay obstaculo, priorizamos el avoidance
        if (_avoidance.TryGetAvoidDir(desiredVelocity, out Vector3 avoidDir))
        {
            desiredVelocity = avoidDir * maxSpeed;
        }

        _velocity = desiredVelocity;

        _rb.linearVelocity = new Vector3(_velocity.x, _rb.linearVelocity.y, _velocity.z);

        Vector3 dirToTarget = _target.position - transform.position;
        dirToTarget.y = 0;

        if (dirToTarget.magnitude > 0.2f)
        {
            Quaternion rot = Quaternion.LookRotation(dirToTarget.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0, euler.y, 0);
    }
    

    public void MoveAwayFromTarget()
    {
        if (_target == null) return;

        Vector3 desiredVelocity =
            (transform.position - _target.position).normalized * maxSpeed;

        if (desiredVelocity.magnitude < 0.05f)
        {
            _velocity = -transform.forward * 0.1f;
            desiredVelocity = _velocity;
        }

        if (_avoidance.TryGetAvoidDir(desiredVelocity, out Vector3 avoidDir))
        {
            desiredVelocity = avoidDir * maxSpeed;
        }

        _velocity = desiredVelocity;

        _rb.linearVelocity =
            new Vector3(_velocity.x, _rb.linearVelocity.y, _velocity.z);

        Vector3 lookDir = -_velocity;
        lookDir.y = 0;

        if (lookDir.magnitude > 0.2f)
        {
            Quaternion rot =
                Quaternion.LookRotation(lookDir.normalized);

            transform.rotation =
                Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0, euler.y, 0);
    }


}