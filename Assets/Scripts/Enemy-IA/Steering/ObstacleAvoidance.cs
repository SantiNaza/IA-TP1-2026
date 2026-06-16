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

        // Detecta obstáculos cerca dentro del radio
        int count = Physics.OverlapSphereNonAlloc(_entity.position, _radius, _colls, _obsMask);

        Collider nearColl = null;
        float nearDist = Mathf.Infinity;
        Vector3 nearPoint = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            Collider c = _colls[i];

            Vector3 closestPoint = c.ClosestPoint(_entity.position);
            closestPoint.y = _entity.position.y; // Mantener en el plano horizontal

            Vector3 dirToColl = closestPoint - _entity.position;
            float dist = dirToColl.magnitude;

            // Evitamos divisiones por cero si la IA está exactamente encima del punto
            if (dist < 0.01f) continue;

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

        // --- NUEVA LÓGICA PARA LABERINTOS Y PAREDES ---

        // Vector que va desde la pared hacia la IA (Fuerza de repulsión directa)
        Vector3 repulsionDir = (_entity.position - nearPoint).normalized;

        // Vector de evasión lateral clásico (el que ya tenías con el producto cruz)
        Vector3 lateralAvoidDir = Vector3.Cross(Vector3.up, (nearPoint - _entity.position).normalized).normalized;

        // Decidir si esquivar por izquierda o derecha según la posición local
        Vector3 local = _entity.InverseTransformPoint(nearPoint);
        if (Mathf.Abs(local.x) > 0.05f)
            _lastSide = (local.x < 0) ? 1 : -1; // Invertido para que coincida con el empuje

        lateralAvoidDir *= _lastSide;

        // COMBINACIÓN CRUCIAL: Sumamos el empuje hacia afuera (repulsionDir) + el esquive lateral (lateralAvoidDir)
        // El factor 1.5f le da prioridad a "despegarse" de la pared antes de seguir avanzando.
        avoidDir = (lateralAvoidDir + repulsionDir * 1.5f).normalized;

        // Debug visual en la escena para la entrega de la UADE (Verde = dirección final de escape)
        Debug.DrawRay(_entity.position, avoidDir * 2f, Color.green);
        // Rojo = Punto exacto de la pared que nos está molestando
        Debug.DrawLine(_entity.position, nearPoint, Color.red);

        return true;
    }
}