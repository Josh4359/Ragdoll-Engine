using UnityEngine;

namespace RagdollEngine
{
    public class FrictionPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float minFriction;

        [SerializeField] float maxFriction;

        [SerializeField] float minHandling;

        [SerializeField] float maxHandling;

        [SerializeField] float maxSpeed;

        public override void Execute()
        {
            if (moving)
            {
                if (accelerationVector.magnitude <= Physics.sleepThreshold) return;

                float handling = Mathf.Lerp(minHandling, maxHandling, 1 - Mathf.Pow(10, -(moveVelocity.magnitude / maxSpeed)));

                accelerationVector += (accelerationVector.normalized * moveVelocity.magnitude * handling)
                    - (moveVelocity * handling);
            }
            else
                additiveVelocity -= moveVelocity * Mathf.Lerp(minFriction, maxFriction, 1 - Mathf.Pow(10, -(moveVelocity.magnitude / maxSpeed)));
        }
    }
}
