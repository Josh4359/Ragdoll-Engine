using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class RollPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float turnSensitivity;

        [SerializeField] float uphillSlopeRatio;

        [SerializeField] float downhillSlopeRatio;

        [HideInInspector] public bool roll;

        bool rollLock;

        void LateUpdate()
        {
            animator.SetBool("Rolling", active);
        }

        public override bool Evaluate()
        {
            rollLock = roll
                || (rollLock
                    && wasActive
                    && groundInformation.ground
                    && !inputHandler.roll.hold);

            roll = false;

            if (((moveVelocity + additiveVelocity).magnitude > moveDeadzone) && (inputHandler.roll.pressed || (wasActive && inputHandler.roll.hold) || rollLock || (wasActive && !groundInformation.ground)))
            {
                if (!wasActive)
                    animator.SetTrigger("Roll");

                return true;
            }

            if (wasActive && groundInformation.ground)
                animator.SetTrigger("Uncurl");

            return false;
        }

        public override void Execute()
        {
            additiveVelocity += Vector3.Project(-Vector3.up * (Vector3.Dot(moveVelocity, Vector3.up) >= 0 ? uphillSlopeRatio : downhillSlopeRatio), moveVelocity.normalized);

            if (inputHandler.move.magnitude <= InputSystem.settings.defaultDeadzoneMin) return;

            Vector3 moveForwardNormal = Vector3.ProjectOnPlane(cameraTransform.forward, playerTransform.up).normalized;

            Vector3 moveRightNormal = Vector3.ProjectOnPlane(cameraTransform.right, playerTransform.up).normalized;

            Vector3 moveNormal = ((moveForwardNormal * inputHandler.move.y) + (moveRightNormal * inputHandler.move.x)) * Mathf.Sign(Vector3.Dot(cameraTransform.up, playerTransform.up));

            Vector3 moveForce = Vector3.Lerp(moveVelocity.normalized, moveNormal, turnSensitivity);

            additiveVelocity += -moveVelocity
                + ((moveForce - Vector3.Project(moveForce, plane)).normalized * moveVelocity.magnitude);
        }
    }
}
