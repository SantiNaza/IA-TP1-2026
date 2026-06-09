using UnityEngine;

public class FlockLeader : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;

    int currentWaypoint;

    void Update()
    {
        if (waypoints.Length == 0)
            return;

        Transform target =
            waypoints[currentWaypoint];

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime);

        if (Vector3.Distance(
                transform.position,
                target.position) < 0.5f)
        {
            currentWaypoint =
                (currentWaypoint + 1)
                % waypoints.Length;
        }
    }
}