using System.Collections.Generic;
using UnityEngine;

public class NavWaypoint : MonoBehaviour
{
    public List<NavWaypoint> neighbors = new List<NavWaypoint>();

    public float CostTo(NavWaypoint other)
    {
        return Vector3.Distance(transform.position, other.transform.position);
    }

    private void OnDrawGizmos()
    {
        bool allBidirectional = true;
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null && !neighbor.neighbors.Contains(this))
            {
                allBidirectional = false;
                break;
            }
        }
        Gizmos.color = allBidirectional ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);

        foreach (var neighbor in neighbors)
        {
            if (neighbor == null) continue;

            bool isReciprocal = neighbor.neighbors.Contains(this);

            Gizmos.color = isReciprocal ? Color.yellow : Color.cyan;

            Vector3 from = transform.position;
            Vector3 to   = neighbor.transform.position;
            Gizmos.DrawLine(from, to);

            Vector3 arrowPos = Vector3.Lerp(from, to, 0.66f);
            Gizmos.DrawSphere(arrowPos, 0.12f);
        }
    }
}