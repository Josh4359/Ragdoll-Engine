using UnityEngine;

namespace RagdollEngine
{
    public class StompPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] AudioSource landAudioSource;

        [SerializeField] float minStompForce;

        [SerializeField] float maxStompForce;

        [SerializeField] float stompAccelerationTime;

        float stompAccelerationTimer;

        void LateUpdate()
        {
            animator.SetBool("Stomping", active);
        }

        public override bool Evaluate()
        {
            bool stomping = (inputHandler.stomp.pressed || wasActive) && !groundInformation.ground;
                

            if (stomping)
            {
                if (wasActive)
                    stompAccelerationTimer -= Time.fixedDeltaTime;
                else
                {
                    stompAccelerationTimer = stompAccelerationTime;

                    animator.SetTrigger("Stomp");
                }
            }
            else if (wasActive && groundInformation.ground)
                landAudioSource.Play();

            return stomping;
        }

        public override void Execute()
        {
            additiveVelocity = -RB.velocity
                + (-Vector3.up * Mathf.Lerp(minStompForce, maxStompForce, 1 - Mathf.Pow(10, -(1 - (stompAccelerationTimer / stompAccelerationTime)))));
        }
    }
}
