using UnityEngine;

public class ObstacleAvoidance
{
    Transform _entity;
    float _radius;
    float _angle;
    LayerMask _obsMask;
    Collider[] _colls;

    int _lastSide = 1; 

    public ObstacleAvoidance(Transform entity, float radius, float angle, LayerMask obsMask, int maxObs = 10)
    {
        _entity = entity;
        _radius = radius;
        _angle = angle;
        _obsMask = obsMask;
        _colls = new Collider[maxObs];
    }

    public bool TryGetAvoidDir(Vector3 currDir, out Vector3 avoidDir)
    {
        avoidDir = Vector3.zero;

        if (currDir == Vector3.zero)
            currDir = _entity.forward;

        // Detecta obstaculos cerca dentro del radio y el angulo (como un cono de vision
        int count = Physics.OverlapSphereNonAlloc(_entity.position, _radius, _colls, _obsMask);

        Collider nearColl = null;
        float nearDist = Mathf.Infinity;
        Vector3 nearPoint = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            Collider c = _colls[i];

            Vector3 closestPoint = c.ClosestPoint(_entity.position);
            closestPoint.y = _entity.position.y;

            Vector3 dirToColl = closestPoint - _entity.position;
            float dist = dirToColl.magnitude;

            float ang = Vector3.Angle(currDir, dirToColl);
            if (ang > _angle * 0.5f) continue;

            if (dist < nearDist)
            {
                nearDist = dist;
                nearColl = c;
                nearPoint = closestPoint;
            }
        }

        if (nearColl == null)
            return false;

        Vector3 dirToClosest = (nearPoint - _entity.position).normalized;

        // Si encuentra un obstaculo adelante, calcula una direccion perpendicular para esquivarlo
        avoidDir = Vector3.Cross(Vector3.up, dirToClosest).normalized;

        // Elege izq o der segun donde este el obstaculo.
        Vector3 local = _entity.InverseTransformPoint(nearPoint);

        if (Mathf.Abs(local.x) > 0.05f)
            _lastSide = (local.x < 0) ? -1 : 1;

        avoidDir *= _lastSide;

        return true;
    }
}