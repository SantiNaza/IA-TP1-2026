using UnityEngine;

public class SafeWaypointManager : MonoBehaviour
{
    public static SafeWaypointManager Instance;

    public NavWaypoint[] safeWaypoints;

    private void Awake()
    {
        Instance = this;
    }
}