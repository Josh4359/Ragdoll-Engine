using UnityEngine;

namespace RagdollEngine
{
    public class HomingAttackPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float speed;

        [SerializeField] float peak;

        [SerializeField] float maxDistance;

        [SerializeField] float viewDot;

        [SerializeField, Range(0, 1)] float maxHeight;

        [SerializeField, Range(0, 1)] float directionToDistancePrioritization;

        [SerializeField] float safeZone;

        HomingTarget target;

        Vector3 start;

        Vector3 point;

        float time;

        float currentTime;

        void LateUpdate()
        {
            animator.SetBool("Homing Attacking", active);

            if (active)
                animator.SetFloat("World Speed", speed);
        }

        public override bool Evaluate()
        {
            if (!wasActive && !inputHandler.attack.pressed) return false;

            if (wasActive) goto Home;

            HomingTarget thisTarget = null;

            Vector3 thisPoint = Vector3.zero;

            float distance = maxDistance;

            float largestRatio = 0;

            foreach (HomingTarget thisHomingTarget in FindObjectsOfType<HomingTarget>())
            {
                if (!thisHomingTarget.Target(playerBehaviourTree, maxDistance, viewDot, maxHeight, out Vector3 thisPoint1)) continue;

                Vector3 difference = thisPoint1 - playerTransform.position;

                float thisDistance = difference.magnitude;

                if (thisDistance > maxDistance || Vector3.Dot(difference.normalized, modelTransform.forward) < viewDot || difference.y / maxDistance > maxHeight) continue;

                bool obstructed = Physics.Raycast(playerTransform.position, difference.normalized, out RaycastHit hit, difference.magnitude - safeZone, layerMask, QueryTriggerInteraction.Ignore);

                float directionFactor = Mathf.Clamp01(Vector3.Dot(difference.normalized, modelTransform.forward));

                float distanceFactor = 1 - (difference.magnitude / maxDistance);

                float directionToDistanceRatio = (directionFactor * directionToDistancePrioritization) + (distanceFactor * (1 - directionToDistancePrioritization));

                if (directionToDistanceRatio > largestRatio//thisDistance < distance
                    && (!obstructed || hit.collider.gameObject == thisHomingTarget.gameObject))
                {
                    thisTarget = thisHomingTarget;

                    thisPoint = thisPoint1;

                    distance = thisDistance;

                    largestRatio = directionToDistanceRatio;
                }
            }

            if (thisTarget)
            {
                target = thisTarget;

                start = playerTransform.position;

                point = thisPoint;

                time = distance;

                currentTime = time;

                animator.SetTrigger("Homing Attack");
            }
            else
                return false;

            target = thisTarget;

        Home:

            float timePercent = Mathf.Clamp01(1 - (currentTime / time));

            Vector3 goal = Vector3.Lerp(start, point, timePercent) + (Vector3.up * Mathf.Sin(timePercent * Mathf.PI) * peak * (Vector3.Distance(start, point) / maxDistance));

            Vector3 difference1 = goal - playerTransform.position;

            if (Physics.Raycast(playerTransform.position, difference1.normalized, difference1.magnitude, layerMask, QueryTriggerInteraction.Ignore)) return false;

            if (timePercent >= 1)
            {
                target.OnTarget(playerBehaviourTree);

                return false;
            }

            RB.position = new Vector3(RB.position.x,
                goal.y,
                RB.position.z);

            kinematic = true;

            movePosition = goal;

            currentTime -= speed * Time.fixedDeltaTime;

            //playerBehaviourTree.groundInformation.ground = false;

            //additiveVelocity = -RB.velocity
            //+ (Vector3.Normalize(point - playerTransform.position) * speed);

            return true;
        }
    }
}
