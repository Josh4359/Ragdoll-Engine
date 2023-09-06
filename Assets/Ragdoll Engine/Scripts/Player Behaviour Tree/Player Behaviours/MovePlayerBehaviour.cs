using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class MovePlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float maxSpeed;

        [SerializeField] float baseSpeed;

        [SerializeField] float acceleration;

        [SerializeField, Range(0, 1)] float smoothness;

        [SerializeField] float uphillSlopeRatio;

        [SerializeField] float downhillSlopeRatio;

        [SerializeField] float maxSpeedUphillSlopeRatio;

        [SerializeField] float maxSpeedDownhillSlopeRatio;

        bool wasMoving;

        public override void Execute()
        {
            active = true;

            wasMoving = wasMoving && wasActive;

            moving = inputHandler.move.magnitude > InputSystem.settings.defaultDeadzoneMin;

            Vector3 moveForwardNormal;

            Vector3 moveRightNormal;

            Vector3 moveNormal;

            moveForwardNormal = Vector3.Cross(cameraTransform.right, playerTransform.up).normalized;

            moveRightNormal = Vector3.Cross(playerTransform.up, cameraTransform.forward).normalized;

            moveNormal = (moveForwardNormal
                    * inputHandler.move.y
                    * Mathf.Sign(Vector3.Dot(tangent, moveForwardNormal)
                        * Mathf.Max(Vector3.Dot(cameraTransform.forward, tangent),
                            -Vector3.Dot(Vector3.Cross(tangent, plane), cameraTransform.up))))
                + (moveRightNormal
                    * inputHandler.move.x
                    * Mathf.Sign(Vector3.Dot(tangent, moveRightNormal)
                        * Mathf.Max(Vector3.Dot(cameraTransform.forward, -plane),
                            -Vector3.Dot(Vector3.Cross(tangent, plane), cameraTransform.up))));

            float speedPercent = Mathf.Min(moveVelocity.magnitude / maxSpeed, 1);

            if (moving)
            {
                if (plane.magnitude > 0)
                {
                    Vector3 axis = Vector3.Cross(plane, playerTransform.up);

                    moveNormal = axis.normalized * Mathf.Sign(Vector3.Dot(moveNormal, axis)) * moveNormal.magnitude;
                }

                float force = moveVelocity.magnitude <= moveDeadzone ? baseSpeed : acceleration * (1 - (Mathf.Sin(Mathf.Clamp01(speedPercent) * (Mathf.PI / 2)) * Mathf.Max(Vector3.Dot(moveNormal.normalized, moveVelocity.normalized), 0)));

                accelerationVector = moveNormal
                    * ((wasMoving && moveVelocity.magnitude > moveDeadzone ? force : force / moveNormal.magnitude) * (1 - Mathf.Max(Vector3.Dot(moveNormal.normalized, Vector3.up), 0)));
            }
            else if (groundInformation.ground && moveVelocity.magnitude <= moveDeadzone)
                additiveVelocity -= moveVelocity;

            if (groundInformation.slope)
                accelerationVector -= Vector3.ProjectOnPlane(groundInformation.hit.normal, playerTransform.up).normalized * Mathf.Min(Vector3.Dot(accelerationVector, Vector3.ProjectOnPlane(groundInformation.hit.normal, playerTransform.up).normalized), 0);

            if (moving || moveVelocity.magnitude > moveDeadzone)
                additiveVelocity += Vector3.Project(-Vector3.up * (Vector3.Dot(moveVelocity, Vector3.up) >= 0 ? Mathf.Lerp(uphillSlopeRatio, maxSpeedUphillSlopeRatio, speedPercent) : Mathf.Lerp(downhillSlopeRatio, maxSpeedDownhillSlopeRatio, speedPercent)), moveVelocity.normalized);

            wasMoving = moving;
        }
    }
}
