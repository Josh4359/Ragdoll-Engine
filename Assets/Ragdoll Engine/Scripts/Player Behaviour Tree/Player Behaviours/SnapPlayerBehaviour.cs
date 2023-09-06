using UnityEngine;

namespace RagdollEngine
{
    public class SnapPlayerBehaviour : PlayerBehaviour
    {
        public override void Execute()
        {
            if (kinematic) return;

            if (groundInformation.ground)
            {
                Vector3 goal = groundInformation.hit.point + (groundInformation.hit.normal * height);

                Vector3 difference = goal - playerTransform.position;

                if (!Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                    playerTransform.position = goal;

                playerTransform.up = groundInformation.hit.normal;
            }
            else
                playerTransform.up = Vector3.up;
        }
    }
}
