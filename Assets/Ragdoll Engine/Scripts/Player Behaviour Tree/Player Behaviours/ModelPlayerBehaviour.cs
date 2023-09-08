using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class ModelPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float groundSmoothness;

        [SerializeField] float airSmoothness;

        [SerializeField] float turnAngle;

        [SerializeField] float turnSmoothness;

        [SerializeField] float maxSpeed;

        public override void Execute()
        {
            float speed = inputHandler.move.magnitude > InputSystem.settings.defaultDeadzoneMin || moveVelocity.magnitude > moveDeadzone ? moveVelocity.magnitude : 0;

            animator.SetBool("Moving", speed > 0);

            animator.SetBool("Ground", groundInformation.ground);

            if (groundInformation.ground)
            {
                animator.SetFloat("Speed", speed);

                animator.SetFloat("World Speed", RB.velocity.magnitude);

                animator.SetFloat("Speed Percent", Mathf.Clamp01(speed / maxSpeed));
            }

            animator.SetFloat("Vertical Velocity", Vector3.Dot(RB.velocity, playerTransform.up));

            float angle = Vector3.SignedAngle(modelTransform.forward, RB.velocity, playerTransform.up) / turnAngle;

            float moveDirection = (1 - Mathf.Pow(10, -Mathf.Abs(angle))) * Mathf.Sign(angle);

            animator.SetFloat("Move Direction", Mathf.Lerp(animator.GetFloat("Move Direction"), moveDirection * (1 - Mathf.Cos(moveDirection / 2 * Mathf.PI)), 1 - turnSmoothness));

            Vector3 up = Vector3.Lerp(modelTransform.up, playerTransform.up, 1 - (groundInformation.ground ? groundSmoothness : airSmoothness));

            Vector3 forward = Vector3.Lerp(modelTransform.forward, moveVelocity, speed > 0 ? (1 - turnSmoothness) : 0);

            if (overrideModelTransform) return;

            modelTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(forward - Vector3.Project(forward, plane), up), up);

            modelTransform.position = groundInformation.cast ? groundInformation.hit.point : playerTransform.position - (modelTransform.up * height);
        }
    }
}
