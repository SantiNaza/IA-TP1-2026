using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    public NavWaypoint[] allWaypoints;

    private void Awake()
    {
        Instance = this;
    }
}