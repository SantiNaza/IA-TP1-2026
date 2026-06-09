using UnityEngine;

public class FlockingGuard : MonoBehaviour
{
    [Header("References")]
    public FlockingManager flock;
    public Transform leader;

    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 5f;

    [Header("Neighbour Detection")]
    public float neighbourRadius = 5f;

    [Header("Leader")]
    public float leaderWeight = 2f;
    public float desiredLeaderDistance = 3f;
    public float leaderSeparationDistance = 2f;

    [Header("Weights")]
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 2f;

    private Vector3 velocity;

    private void Update()
    {
        if (flock == null)
            return;

        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();

        // --------------------------
        // FOLLOW LEADER
        // --------------------------

        Vector3 leaderForce = Vector3.zero;

        if (leader != null)
        {
            float distToLeader =
                Vector3.Distance(transform.position, leader.position);

            // Solo lo sigue si está lejos
            if (distToLeader > desiredLeaderDistance)
            {
                leaderForce =
                    (leader.position - transform.position).normalized;
            }

            // Evita superponerse con él
            if (distToLeader < leaderSeparationDistance)
            {
                separation +=
                    (transform.position - leader.position).normalized
                    * (leaderSeparationDistance - distToLeader);
            }
        }

        // --------------------------
        // COMBINAR FUERZAS
        // --------------------------

        Vector3 flockDirection =
            alignment * alignmentWeight +
            cohesion * cohesionWeight +
            separation * separationWeight +
            leaderForce * leaderWeight;

        flockDirection.y = 0;

        if (flockDirection.magnitude > 0.01f)
        {
            velocity = flockDirection.normalized;

            transform.position +=
                velocity *
                speed *
                Time.deltaTime;

            Quaternion targetRot =
                Quaternion.LookRotation(velocity);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime);
        }
    }

    Vector3 CalculateAlignment()
    {
        Vector3 avgDir = Vector3.zero;
        int count = 0;

        foreach (var guard in flock.guards)
        {
            if (guard == this)
                continue;

            float dist =
                Vector3.Distance(
                    transform.position,
                    guard.transform.position);

            if (dist < neighbourRadius)
            {
                avgDir += guard.velocity;
                count++;
            }
        }

        if (count == 0)
            return transform.forward;

        return (avgDir / count).normalized;
    }

    Vector3 CalculateCohesion()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var guard in flock.guards)
        {
            if (guard == this)
                continue;

            float dist =
                Vector3.Distance(
                    transform.position,
                    guard.transform.position);

            if (dist < neighbourRadius)
            {
                center += guard.transform.position;
                count++;
            }
        }

        if (count == 0)
            return Vector3.zero;

        center /= count;

        return (center - transform.position).normalized;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 force = Vector3.zero;

        foreach (var guard in flock.guards)
        {
            if (guard == this)
                continue;

            float dist =
                Vector3.Distance(
                    transform.position,
                    guard.transform.position);

            if (dist < neighbourRadius && dist > 0)
            {
                force +=
                    (transform.position -
                     guard.transform.position)
                    / dist;
            }
        }

        return force.normalized;
    }
}