using UnityEngine;

// Enum con los estados principales que tiene el Guard dentro de la FSM.

public enum GuardStateEnum
{
    Patrol,
    Idle,
    Chase,
    Alert,
    Pathfinding,
    Attack
}