using UnityEngine;

namespace RagdollEngine
{
    public class SpringPlayerBehaviour : PlayerBehaviour
    {
        SpringStageObject springStageObject;

        bool spring;

        float currentLength;

        float speed;

        void LateUpdate()
        {
            spring = active
                || (spring
                    && !groundInformation.ground
                    && RB.velocity.y >= 0);

            animator.SetBool("Springing", spring);
        }

        public override bool Evaluate()
        {
            if (!SpringCheck()) return false;

            Vector3 goal = springStageObject.transform.position + (springStageObject.transform.up * (springStageObject.length - currentLength));

            movePosition = goal;

            kinematic = true;

            modelTransform.rotation = Quaternion.LookRotation(springStageObject.transform.forward, springStageObject.transform.up);

            modelTransform.position = goal - (modelTransform.up * height);

            overrideModelTransform = true;

            currentLength = Mathf.Max(currentLength - (Mathf.Lerp(springStageObject.speed, speed, Vector3.Dot(RB.velocity, springStageObject.transform.up) > 0 ? RB.velocity.magnitude : 0) * Time.fixedDeltaTime), 0);

            if (currentLength <= 0)
                return false;

            return true;
        }

        bool SpringCheck()
        {
            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is SpringStageObject)
                {
                    if (wasActive && thisStageObject == springStageObject) return true;

                    springStageObject = thisStageObject as SpringStageObject;

                    springStageObject.audioSource.Play();

                    Vector3 goal = playerTransform.position - Vector3.ProjectOnPlane(playerTransform.position - thisStageObject.transform.position, thisStageObject.transform.up);

                    Vector3 difference = playerTransform.position - goal;

                    if (!Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                        playerTransform.position = goal;

                    currentLength = springStageObject.length;

                    speed = wasActive
                        ? Mathf.Max(springStageObject.speed, speed)
                        : Vector3.Dot(RB.velocity, springStageObject.transform.up) > 0
                            ? Mathf.Max(springStageObject.speed, RB.velocity.magnitude)
                            : springStageObject.speed;

                    additiveVelocity = -RB.velocity
                        + thisStageObject.transform.up * (Vector3.Dot(RB.velocity, thisStageObject.transform.up) > 0 ? Mathf.Max(springStageObject.speed, RB.velocity.magnitude) : springStageObject.speed);

                    animator.SetTrigger("Spring");

                    return true;
                }

            if (wasActive && currentLength > 0) return true;

            return false;
        }
    }

    /*
    [SerializeField] GravityPlayerBehaviour gravityPlayerBehaviour;

    public override bool Evaluate()
    {
        foreach (StageObject thisStageObject in stageObjects)
            if (thisStageObject is SpringStageObject)
            {
                playerBehaviourTree.groundInformation.ground = false;

                if (wasActive) return true;

                SpringStageObject springStageObject = thisStageObject as SpringStageObject;

                springStageObject.audioSource.Play();

                Vector3 displacement = springStageObject.transform.position + springStageObject.goal - playerTransform.position;

                float gravity = gravityPlayerBehaviour.gravity / Time.fixedDeltaTime;

                additiveVelocity = -RB.velocity
                    + (Vector3.up * Mathf.Sqrt(2 * gravity * springStageObject.height))
                    + (Vector3.ProjectOnPlane(displacement, Vector3.up) / (Mathf.Sqrt(2 * springStageObject.height / gravity) + Mathf.Sqrt(-2 * (displacement.y - springStageObject.height) / gravity)));

                springStageObject.Spring();

                return true;
            }

        return false;
    }
    */
}
