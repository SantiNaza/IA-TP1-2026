using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public List<FlockingGuard> guards = new List<FlockingGuard>();

    private void Awake()
    {
        guards.Clear();

        FlockingGuard[] allGuards = FindObjectsByType<FlockingGuard>(FindObjectsSortMode.None);
        
        guards.AddRange(allGuards);
    }
}