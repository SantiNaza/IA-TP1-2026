using UnityEngine;


public static class SteeringBehaviours
{
    // =====================================================
    // SEEK
    // Ir hacia un objetivo desacelerando al aproximarse.
    // =====================================================
    public static Vector3 Seek(Transform agent, Vector3 targetPos, float maxSpeed)
    {
        Vector3 desired = targetPos - agent.position;
        desired.y = 0f;

        if (desired == Vector3.zero)
            return Vector3.zero;

        return desired.normalized * maxSpeed;
    }

    // =====================================================
    // FLEE
    // Alejarse del objetivo.
    // =====================================================
    public static Vector3 Flee(Transform agent, Transform target, float maxSpeed)
    {
        Vector3 desired = agent.position - target.position;
        desired.y = 0f;

        if (desired == Vector3.zero)
            return Vector3.zero;

        return desired.normalized * maxSpeed;
    }

    // =====================================================
    // PURSUIT
    // Perseguir prediciendo posición futura del jugador.
    // =====================================================
    public static Vector3 Pursuit(Transform agent, Transform target, float maxSpeed, float timePrediction = 0.7f)
    {
        Vector3 futurePos = target.position;

        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb != null)
            futurePos += rb.linearVelocity * timePrediction;

        Vector3 desired = futurePos - agent.position;
        desired.y = 0f;

        if (desired == Vector3.zero)
            return Vector3.zero;

        return desired.normalized * maxSpeed;
    }

    // =====================================================
    // EVADE
    // Escapar prediciendo hacia dónde va el jugador.
    // =====================================================
    public static Vector3 Evade(Transform agent, Transform target, float maxSpeed, float timePrediction = 0.7f)
    {
        Vector3 futurePos = target.position;

        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb != null)
            futurePos += rb.linearVelocity * timePrediction;

        Vector3 desired = agent.position - futurePos;
        desired.y = 0f;

        if (desired == Vector3.zero)
            return Vector3.zero;

        return desired.normalized * maxSpeed;
    }

    // =====================================================
    // OBSTACLE AVOIDANCE
    // Si detecta pared adelante, esquiva hacia un costado.
    // =====================================================
    public static Vector3 ObstacleAvoidance(Transform agent, float detectDistance = 2f)
    {
        RaycastHit hit;

        Vector3 origin = agent.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, agent.forward, out hit, detectDistance))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                Vector3 avoidDir = Vector3.Cross(hit.normal, Vector3.up);
                avoidDir.y = 0f;

                return avoidDir.normalized;
            }
        }

        return Vector3.zero;
    }

    // =====================================================
    // COMBINA DOS FUERZAS
    // =====================================================
    public static Vector3 Combine(Vector3 mainForce, Vector3 avoidForce)
    {
        Vector3 result = mainForce + avoidForce;

        result.y = 0f;

        if (result == Vector3.zero)
            return Vector3.zero;

        return result.normalized;
    }
}