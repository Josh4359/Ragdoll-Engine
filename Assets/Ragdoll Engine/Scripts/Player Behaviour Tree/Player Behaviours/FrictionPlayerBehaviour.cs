using UnityEngine;

namespace RagdollEngine
{
    public class FrictionPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] AnimationCurve handlingOverSpeed = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] AnimationCurve airHandlingOverSpeed = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] float minFriction;

        [SerializeField] float maxFriction;

        [SerializeField] float minHandling;

        [SerializeField] float maxHandling;

        [SerializeField] float minBrake;

        [SerializeField] float maxBrake;

        [SerializeField] float minBrakeSpeed;

        [SerializeField] float maxSpeed;

        bool braking;

        void LateUpdate()
        {
            animator.SetBool("Braking", braking && active);
        }

        public override void Execute()
        {
            if (braking)
                braking = wasActive && groundInformation.ground && RB.velocity.magnitude > moveDeadzone;

            if (!braking)
            {
                braking = groundInformation.ground && RB.velocity.magnitude > minBrakeSpeed && moving && Vector3.Dot(accelerationVector.normalized, RB.velocity.normalized) < 0;

                if (braking)
                    animator.SetTrigger("Brake");
                else
                {
                    if (moving)
                        Turn();
                    else
                        Slow();
                }
            }

            if (braking)
                Brake();

            active = true;
        }

        void Turn()
        {
            if (accelerationVector.magnitude <= Physics.sleepThreshold) return;

            float handling = Mathf.Lerp(minHandling, maxHandling, (groundInformation.ground ? handlingOverSpeed : airHandlingOverSpeed).Evaluate(1 - Mathf.Pow(10, -(moveVelocity.magnitude / maxSpeed))));

            accelerationVector += (accelerationVector.normalized * moveVelocity.magnitude * handling)
                - (moveVelocity * handling);
        }

        void Slow()
        {
            additiveVelocity -= moveVelocity * Mathf.Lerp(minFriction, maxFriction, 1 - Mathf.Pow(10, -(moveVelocity.magnitude / maxSpeed)));
        }

        void Brake()
        {
            additiveVelocity -= moveVelocity * Mathf.Lerp(minBrake, maxBrake, 1 - Mathf.Pow(10, -(moveVelocity.magnitude / maxSpeed)));

            accelerationVector = Vector3.zero;
        }
    }
}
