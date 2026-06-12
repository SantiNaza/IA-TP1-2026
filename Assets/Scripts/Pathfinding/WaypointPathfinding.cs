using System.Collections.Generic;
using UnityEngine;

public static class WaypointPathfinding
{
    public static List<NavWaypoint> FindPath(
        NavWaypoint start,
        NavWaypoint goal)
    {
        Queue<NavWaypoint> frontier = new Queue<NavWaypoint>();

        Dictionary<NavWaypoint, NavWaypoint> cameFrom =
            new Dictionary<NavWaypoint, NavWaypoint>();

        frontier.Enqueue(start);
        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            NavWaypoint current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (NavWaypoint next in current.neighbors)
            {
                if (cameFrom.ContainsKey(next))
                    continue;

                frontier.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        List<NavWaypoint> path = new List<NavWaypoint>();

        if (!cameFrom.ContainsKey(goal))
            return path;

        NavWaypoint currentNode = goal;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }

        path.Reverse();

        return path;
    }
}