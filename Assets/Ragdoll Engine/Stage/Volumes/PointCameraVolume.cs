using UnityEngine;

public class PointCameraVolume : Volume
{
    public Vector3 point;

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(point, 1);
    }
}
