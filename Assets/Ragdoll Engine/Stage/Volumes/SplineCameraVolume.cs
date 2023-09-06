using UnityEngine;
using UnityEngine.Splines;

public class SplineCameraVolume : Volume
{
    public SplineContainer projectedSplineContainer;

    public SplineContainer cameraSplineContainer;

    public Vector3 rotation;

    public bool followPlayer;

    [Range(0, 1)] public float clampMin = 0;

    [Range(0, 1)] public float clampMax = 1;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, Quaternion.Euler(rotation) * Vector3.right * 10);

        Gizmos.color = Color.green;

        Gizmos.DrawRay(transform.position, Quaternion.Euler(rotation) * Vector3.up * 10);

        Gizmos.color = Color.blue;

        Gizmos.DrawRay(transform.position, Quaternion.Euler(rotation) * Vector3.forward * 10);

        if (!projectedSplineContainer) return;

        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(projectedSplineContainer.EvaluatePosition(clampMin), 1);

        Gizmos.DrawWireSphere(projectedSplineContainer.EvaluatePosition(clampMax), 1);
    }
}
