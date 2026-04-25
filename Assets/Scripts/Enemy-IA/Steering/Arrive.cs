using UnityEngine;

public class Arrive : ISteering
{
    private Transform _target;
    private Transform _entity;

    private float _maxSpeed;
    private float _slowingDistance;

    // Arrive: igual que Seek pero reduce la velocidad cuando esta cerca del objetivo.

    public Arrive(Transform target, Transform entity, float maxSpeed, float slowingDistance)
    {
        _target = target;
        _entity = entity;
        _maxSpeed = maxSpeed;
        _slowingDistance = slowingDistance;
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        Vector3 dir = _target.position - _entity.position;
        float distance = dir.magnitude;

        // Si ya llegamos, devolvemos 0 para que frene
        if (distance <= 1f)
            return Vector3.zero;

        float rampedSpeed = _maxSpeed * (distance / _slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, _maxSpeed);

        Vector3 desired = (dir / distance) * clippedSpeed;
        Vector3 steer = desired - currentSpeed;

        currentSpeed += steer * Time.deltaTime;

        return currentSpeed;
    }
}