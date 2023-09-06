using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineAttach : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;

    [SerializeField] Vector3 offset;

    [SerializeField] float distance;

    void Update()
    {
        if (!splineContainer) return;

        splineContainer.Evaluate(distance / splineContainer.CalculateLength(), out float3 position, out float3 tangent, out float3 upVector);

        Vector3 forward = Vector3.Normalize(tangent);

        Vector3 up = Vector3.Normalize(upVector);

        Vector3 right = Vector3.Cross(forward, up);

        transform.position = (Vector3)position
            + (right * offset.x)
            + (up * offset.y)
            + (forward * offset.z);

        transform.rotation = Quaternion.LookRotation(forward, up);
    }
}
