using UnityEngine;

namespace RagdollEngine
{
    public class DashRingPlayerBehaviour : PlayerBehaviour
    {
        DashRingStageObject dashRingStageObject;

        bool dashRing;

        float currentLength;

        float speed;

        void LateUpdate()
        {
            dashRing = active
                || (dashRing
                    && !groundInformation.ground
                    && RB.velocity.y >= 0);

            animator.SetBool("Dash Ringing", dashRing);
        }

        public override bool Evaluate()
        {
            if (!DashRingCheck()) return false;

            Vector3 goal = dashRingStageObject.transform.position + (dashRingStageObject.transform.forward * (dashRingStageObject.length - currentLength));

            movePosition = goal;

            kinematic = true;

            modelTransform.rotation = Quaternion.LookRotation(-dashRingStageObject.transform.up, dashRingStageObject.transform.forward);

            modelTransform.position = goal - (modelTransform.up * height);

            overrideModelTransform = true;

            currentLength = Mathf.Max(currentLength - (Mathf.Lerp(dashRingStageObject.speed, speed, Vector3.Dot(RB.velocity, dashRingStageObject.transform.forward) > 0 ? RB.velocity.magnitude : 0) * Time.fixedDeltaTime), 0);

            if (currentLength <= 0)
                return false;

            return true;
        }

        bool DashRingCheck()
        {
            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is DashRingStageObject)
                {
                    if (wasActive && thisStageObject == dashRingStageObject) return true;

                    dashRingStageObject = thisStageObject as DashRingStageObject;

                    dashRingStageObject.audioSource.Play();

                    Vector3 goal = playerTransform.position - Vector3.ProjectOnPlane(playerTransform.position - thisStageObject.transform.position, thisStageObject.transform.forward);

                    Vector3 difference = playerTransform.position - goal;

                    if (!Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                        playerTransform.position = goal;

                    currentLength = dashRingStageObject.length;

                    speed = wasActive
                        ? Mathf.Max(dashRingStageObject.speed, speed)
                        : Vector3.Dot(RB.velocity, dashRingStageObject.transform.forward) > 0
                            ? Mathf.Max(dashRingStageObject.speed, RB.velocity.magnitude)
                            : dashRingStageObject.speed;

                    additiveVelocity = -RB.velocity
                        + thisStageObject.transform.forward * (Vector3.Dot(RB.velocity, thisStageObject.transform.forward) > 0 ? Mathf.Max(dashRingStageObject.speed, RB.velocity.magnitude) : dashRingStageObject.speed);

                    animator.SetTrigger("Dash Ring");

                    return true;
                }

            if (wasActive && currentLength > 0) return true;

            return false;
        }
    }
}
