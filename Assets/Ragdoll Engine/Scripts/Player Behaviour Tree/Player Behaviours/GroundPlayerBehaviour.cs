using UnityEngine;

namespace RagdollEngine
{
    public class GroundPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float groundDistance;

        [SerializeField] float slopeLimit;

        [SerializeField] float slopeCooldownLimit;

        [SerializeField] float localSlopeLimit;

        [SerializeField] float slopeCooldownTime;

        [SerializeField] float loopSpeed;

        bool slopeCooldown;

        public override bool Evaluate()
        {
            bool cast = Physics.Raycast(playerTransform.position,
                -playerTransform.up,
                out RaycastHit hit,
                (groundInformation.ground ? height + groundDistance : height)
                    + Mathf.Max(Vector3.Dot(RB.velocity, -playerTransform.up) * Time.fixedDeltaTime, 0)
                    + Physics.defaultContactOffset,
                layerMask,
                QueryTriggerInteraction.Ignore);

            bool slope = cast
                && (Vector3.Angle(hit.normal, Vector3.up) > (slopeCooldown ? slopeCooldownLimit : Mathf.Lerp(slopeLimit, 180, Mathf.Clamp(RB.velocity.magnitude / (Vector3.Dot(hit.normal, Vector3.up) >= 0 ? moveDeadzone : loopSpeed), 0, 1)))
                    || Vector3.Angle(hit.normal, playerTransform.up) > localSlopeLimit);

            if (cast && slope)
                slopeCooldown = true;

            bool ground = cast
                && !slope
                && (groundInformation.ground || Vector3.Dot(RB.velocity, hit.normal) <= 0);

            if (ground && slopeCooldown)
                slopeCooldown = false;

            groundInformation = new PlayerBehaviourTree.GroundInformation()
            {
                hit = hit,

                ground = ground,

                cast = cast,

                slope = slope
            };

            return ground;
        }
    }
}
