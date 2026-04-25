using UnityEngine;

public class Seek : ISteering
{
    Transform _target;
    Transform _entity;
    float _maxSpeed;

    // Seek: intenta moverse hacia el objetivo a velocidad maxima. 

    public Seek(Transform target, Transform entity, float maxSpeed)
    {
        _target = target;
        _entity = entity;
        _maxSpeed = maxSpeed;
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        Vector3 dir = (_target.position - _entity.position).normalized;
        Vector3 desired = dir * _maxSpeed;
        Vector3 steer = desired - currentSpeed;

        currentSpeed += steer * Time.deltaTime;
        return currentSpeed;
    }
}