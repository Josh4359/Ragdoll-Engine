using UnityEngine;

namespace RagdollEngine
{
    public class FollowPlayerCameraState : PlayerCameraState
    {
        [Header("Zoom")]

        [SerializeField] float defaultDistance;

        [SerializeField] float minDistance;

        [SerializeField] float maxDistance;

        [SerializeField] float zoomSensitivity;

        [SerializeField] float zoomFalloff;

        [SerializeField, Range(0, 1)] float zoomSmoothness;

        float distance;

        float currentDistance;

        [Header("Follow")]

        [SerializeField] Vector2 followIntensity;

        [SerializeField] float maxFollowSpeed;

        [Header("Normal")]

        [SerializeField, Range(0, 1)] float normalSmoothness;

        Vector3 normal;

        [Header("Rotation")]

        [SerializeField] Vector2 defaultRotation;

        [SerializeField] float lookSensitivity;

        Quaternion rotation = Quaternion.identity;

        Vector2 lookRotation;

        Vector2 look;

        [Header("Ghost")]

        [SerializeField, Range(0, 1)] float ghostSmoothnessXZ;

        [SerializeField, Range(0, 1)] float ghostSmoothnessY;

        [SerializeField] float maxGhostDistanceMinXZ;

        [SerializeField] float maxGhostDistanceMinY;

        [SerializeField] float maxGhostDistanceMaxXZ;

        [SerializeField] float maxGhostDistanceMaxY;

        Vector3 ghostOffset;

        Vector3 ghostPosition;

        [Header("Offset")]

        [SerializeField] float heightOffset;

        void Awake()
        {
            distance = defaultDistance;
        }

        void Update()
        {
            look += inputHandler.lookDelta;

            distance = CalculateDistance(inputHandler.zoomDelta.value);
        }

        public override void Execute()
        {
            // Input

            if (transition <= 0)
                look += inputHandler.look / Time.fixedDeltaTime;

            distance = CalculateDistance(inputHandler.zoom.value / Time.fixedDeltaTime);

            // Zoom

            currentDistance = Mathf.Lerp(currentDistance, distance, 1 - zoomSmoothness);

            // Follow

            if (transition <= 0 && look.magnitude == 0)
            {
                float followPercent = Mathf.Min(RB.velocity.magnitude / maxFollowSpeed, 1);

                look.x += Vector3.Dot(Vector3.ProjectOnPlane(cameraTransform.right, normal).normalized, RB.velocity.normalized) * followPercent * followIntensity.x;

                look.y += Mathf.Max((Vector3.Dot(cameraTransform.up, normal) + defaultRotation.y) * followPercent * followIntensity.y, Mathf.Min(-lookRotation.y, 0));
            }

            // Normal
            normal = Vector3.Lerp(normal,
                    groundInformation.ground ? groundInformation.hit.normal : Vector3.up,
                    1 - normalSmoothness);

            // Rotation

            lookRotation += look * lookSensitivity;

            lookRotation.y = Mathf.Clamp(lookRotation.y, -89, 89);

            look = Vector2.zero;

            Vector3 forward = Quaternion.AngleAxis(lookRotation.x, Vector3.up) * Vector3.forward;

            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            Vector3 up = Quaternion.AngleAxis(-lookRotation.y, right) * Vector3.up;

            forward = Vector3.ProjectOnPlane(forward, up).normalized;

            rotation = Quaternion.LookRotation(Vector3.Distance(rotation * Vector3.forward, normal) > 0
                    ? Vector3.ProjectOnPlane(rotation * Vector3.forward, normal)
                    : Quaternion.FromToRotation(Vector3.up, normal) * Vector3.up,
                normal);

            Quaternion goalRotation = rotation * Quaternion.LookRotation(forward, up);

            // Ghost

            ghostOffset += modelTransform.position - ghostPosition;

            ghostOffset -= Vector3.ProjectOnPlane(ghostOffset, normal) * (1 - ghostSmoothnessXZ);

            Vector3 ghostOffsetXZ = Vector3.ProjectOnPlane(ghostOffset, normal);

            ghostOffset -= ghostOffsetXZ.normalized * Mathf.Max(ghostOffsetXZ.magnitude - Mathf.Lerp(maxGhostDistanceMinXZ, maxGhostDistanceMaxXZ, (currentDistance - minDistance) / maxDistance), 0);

            ghostOffset -= Vector3.Project(ghostOffset, normal) * (1 - ghostSmoothnessY);

            Vector3 ghostOffsetY = Vector3.Project(ghostOffset, normal);

            ghostOffset -= ghostOffsetY.normalized * Mathf.Max(ghostOffsetY.magnitude - Mathf.Lerp(maxGhostDistanceMinY, maxGhostDistanceMaxY, (currentDistance - minDistance) / maxDistance), 0);

            ghostPosition = modelTransform.position;

            // Position

            Vector3 goalPosition = modelTransform.position
                + (goalRotation * -Vector3.forward * currentDistance)
                + (normal * heightOffset)
                - ghostOffset;

            // Apply

            cameraTransform.position = modelTransform.position
                + Vector3.Slerp(oldPosition,
                    goalPosition - modelTransform.position,
                    Mathf.SmoothStep(1, 0, transition / transitionTime));

            cameraTransform.rotation = Quaternion.Slerp(oldRotation,
                goalRotation,
                1 - Mathf.Sin(Mathf.SmoothStep(0, 1, transition / transitionTime) * Mathf.PI / 2));
        }

        public override void Enable()
        {
            look = Vector2.zero;

            lookRotation = defaultRotation;

            normal = groundInformation.ground ? groundInformation.hit.normal : Vector3.up;

            rotation = cameraTransform.rotation;

            ghostOffset = Vector3.zero;

            ghostPosition = modelTransform.position;

            distance = defaultDistance;

            currentDistance = distance;
        }

        public override void Transition()
        {
            base.Transition();

            Enable();

            rotation = Quaternion.identity;

            lookRotation.x = Vector3.SignedAngle(modelTransform.forward, Vector3.forward, -Vector3.up);
        }

        float CalculateDistance(float delta)
        {
            return Mathf.Clamp(distance
                    + (delta
                        * zoomSensitivity
                        * Mathf.Clamp01(Mathf.Max((maxDistance - distance) / zoomFalloff, -Mathf.Sign(delta * zoomSensitivity)))
                        * Mathf.Clamp01(Mathf.Max((distance - minDistance) / zoomFalloff, Mathf.Sign(delta * zoomSensitivity)))),
                minDistance,
                maxDistance);
        }
    }
}
