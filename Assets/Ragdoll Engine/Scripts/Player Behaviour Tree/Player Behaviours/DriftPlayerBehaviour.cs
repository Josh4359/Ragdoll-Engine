using UnityEngine;

namespace RagdollEngine
{
    public class DriftPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float minTurnSpeed;

        [SerializeField] float maxTurnSpeed;

        [SerializeField] float centrifugalForce;

        [SerializeField] float transitionSmoothness;

        int driftDirection;

        void LateUpdate()
        {
            animator.SetBool("Drifting", active);
        }

        public override bool Evaluate()
        {
            return (inputHandler.roll.pressed || (wasActive && inputHandler.roll.hold)) && !(plane.magnitude > 0);
        }

        public override void Execute()
        {
            if (!active) return;

            Vector3 moveForwardNormal = Quaternion.AngleAxis(Mathf.Lerp(minTurnSpeed, maxTurnSpeed, Mathf.Abs((inputHandler.move.x + driftDirection) / 2)) * driftDirection * Time.fixedDeltaTime, Vector3.up) * moveVelocity.normalized;

            Vector3 moveRightNormal = Vector3.Cross(playerTransform.up, moveForwardNormal).normalized;

            if (!wasActive)
            {
                animator.SetTrigger("Drift");

                driftDirection = Mathf.RoundToInt(Mathf.Sign(inputHandler.move.x * Vector3.Dot(cameraTransform.forward, moveForwardNormal)));
            }

            animator.SetFloat("Drift Direction", Mathf.Lerp(animator.GetFloat("Drift Direction"), driftDirection, 1 - transitionSmoothness));

            additiveVelocity = -moveVelocity
                + (moveForwardNormal * moveVelocity.magnitude)
                 + (-moveRightNormal * centrifugalForce * driftDirection);
        }
    }
}
