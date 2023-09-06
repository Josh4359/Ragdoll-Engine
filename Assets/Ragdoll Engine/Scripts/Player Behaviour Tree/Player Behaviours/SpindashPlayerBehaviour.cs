using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    [RequireComponent(typeof(RollPlayerBehaviour))]
    public class SpindashPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] AnimationCurve force;

        [SerializeField] float forceIntensity;

        [SerializeField] float chargeTime;

        RollPlayerBehaviour rollPlayerBehaviour;

        float currentChargeTime;

        void Awake()
        {
            rollPlayerBehaviour = GetComponent<RollPlayerBehaviour>();
        }

        void LateUpdate()
        {
            animator.SetBool("Spindashing", active);
        }

        public override bool Evaluate()
        {
            if (inputHandler.stomp.pressed)
            {
                currentChargeTime = chargeTime;

                rollPlayerBehaviour.roll = false;

                animator.SetTrigger("Spindash");

                return true;
            }

            if (wasActive && !inputHandler.stomp.hold)
            {
                additiveVelocity = modelTransform.forward *
                    force.Evaluate(Mathf.Clamp01(moveVelocity.magnitude / forceIntensity))
                     * ((chargeTime - currentChargeTime) / chargeTime)
                     * forceIntensity;

                rollPlayerBehaviour.roll = true;

                animator.SetTrigger("Roll");

                return false;
            }

            return wasActive;
        }

        public override void Execute()
        {
            if (currentChargeTime > 0)
                currentChargeTime -= Time.fixedDeltaTime;

            if (currentChargeTime < 0)
                currentChargeTime = 0;

            overrideModelTransform = true;

            modelTransform.position = groundInformation.ground ? groundInformation.hit.point : playerTransform.position - (modelTransform.up * height);

            if (inputHandler.move.magnitude > InputSystem.settings.defaultDeadzoneMin)
            {
                Vector3 moveForwardNormal = Vector3.Cross(cameraTransform.right, playerTransform.up).normalized;

                Vector3 moveRightNormal = Vector3.Cross(playerTransform.up, cameraTransform.forward).normalized;

                Vector3 moveNormal = (moveForwardNormal * inputHandler.move.y) + (moveRightNormal * inputHandler.move.x);

                modelTransform.rotation = Quaternion.LookRotation(moveNormal, playerTransform.up);
            }

            accelerationVector = Vector3.zero;
        }
    }
}
