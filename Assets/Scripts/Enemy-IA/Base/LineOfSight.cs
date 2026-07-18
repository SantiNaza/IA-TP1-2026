using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float range = 10f;
    public float angle = 90f;
    public LayerMask obstacleMask;

    public bool CanSeeTarget(Transform target)
    {
        if (target == null) return false;

        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude;

        if (dist > range) return false;

        dir.y = 0;
        float ang = Vector3.Angle(transform.forward, dir);
        if (ang > angle / 2f) return false;

        if (Physics.Raycast(transform.position, dir.normalized, dist, obstacleMask))
            return false;

        return true;
    }

    public bool HasClearPath(Vector3 destination)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir    = destination - origin;
        float   dist   = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, dist, obstacleMask))
        {
            Debug.DrawRay(origin, dir.normalized * dist, Color.red, 0.1f);
            return false;
        }

        Debug.DrawRay(origin, dir.normalized * dist, Color.green, 0.1f);
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.red;
        Vector3 right = Quaternion.Euler(0,  angle / 2f, 0) * transform.forward;
        Vector3 left  = Quaternion.Euler(0, -angle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, right * range);
        Gizmos.DrawRay(transform.position, left  * range);
    }
}