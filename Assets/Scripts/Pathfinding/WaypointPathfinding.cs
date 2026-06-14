using System.Collections.Generic;
using UnityEngine;

public static class WaypointPathfinding
{
    public static List<NavWaypoint> FindPath(NavWaypoint start,NavWaypoint goal)
    {
        List<NavWaypoint> openSet = new List<NavWaypoint>();

        Dictionary<NavWaypoint, NavWaypoint> cameFrom = new Dictionary<NavWaypoint, NavWaypoint>();

        Dictionary<NavWaypoint, float> gScore = new Dictionary<NavWaypoint, float>();

        Dictionary<NavWaypoint, float> fScore = new Dictionary<NavWaypoint, float>();

        openSet.Add(start);

        gScore[start] = 0;

        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            NavWaypoint current = GetLowestFScore(openSet, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (NavWaypoint neighbor in current.neighbors)
            {
                float tentativeG = gScore[current] + current.CostTo(neighbor);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;

                    gScore[neighbor] = tentativeG;

                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<NavWaypoint>();
    }

    static float Heuristic( NavWaypoint a, NavWaypoint b)
    {
        return Vector3.Distance( a.transform.position, b.transform.position);
    }

    static NavWaypoint GetLowestFScore( List<NavWaypoint> openSet, Dictionary<NavWaypoint, float> fScore)
    {
        NavWaypoint best = openSet[0];

        float bestScore = fScore[best];

        foreach (NavWaypoint node in openSet)
        {
            if (fScore[node] < bestScore)
            {
                best = node;
                bestScore = fScore[node];
            }
        }

        return best;
    }

    static List<NavWaypoint> ReconstructPath( Dictionary<NavWaypoint, NavWaypoint> cameFrom, NavWaypoint current)
    {
        List<NavWaypoint> path = new List<NavWaypoint>();

        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();

        return path;
    }
}