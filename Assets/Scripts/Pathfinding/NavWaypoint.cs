using System.Collections.Generic;
using UnityEngine;

public class NavWaypoint : MonoBehaviour
{
    public List<NavWaypoint> neighbors = new List<NavWaypoint>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.yellow;

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    neighbor.transform.position);
            }
        }
    }
}