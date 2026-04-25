using UnityEngine;

public interface ISteering
{
    Vector3 GetDir(Vector3 currentSpeed);
}