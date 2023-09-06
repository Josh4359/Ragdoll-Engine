using UnityEngine;

namespace RagdollEngine
{
    public class UpreelPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float cooldown;

        UpreelStageObject upreelStageObject;

        bool upreel;

        float currentCooldown;

        public override bool Evaluate()
        {
            if (upreel)
            {
                if (!upreelStageObject.extended)
                {
                    upreel = false;

                    currentCooldown = cooldown;

                    animator.SetBool("Upreeling", false);

                    additiveVelocity = -RB.velocity
                        + (Vector3.up * upreelStageObject.retractionSpeed);

                    return false;
                }

                Upreel();

                return true;
            }

            if (currentCooldown > 0)
            {
                currentCooldown = groundInformation.ground ? 0 : Mathf.Max(currentCooldown - Time.fixedDeltaTime, 0);

                if (currentCooldown > 0)
                    return false;
            }

            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is UpreelStageObject)
                {
                    UpreelStageObject thisUpreelStageObject = thisStageObject as UpreelStageObject;

                    Enter(thisUpreelStageObject);

                    return true;
                }

            return false;
        }

        void Upreel()
        {
            upreelStageObject.UpdatePosition();

            kinematic = true;

            movePosition = upreelStageObject.transform.position;

            modelTransform.position = upreelStageObject.transform.position;

            modelTransform.rotation = upreelStageObject.transform.rotation;
        }

        public void Enter(UpreelStageObject upreelStageObject)
        {
            if (!upreelStageObject.extended) return;

            this.upreelStageObject = upreelStageObject;

            upreelStageObject.Retract();

            upreel = true;

            Upreel();

            animator.SetTrigger("Upreel");

            animator.SetBool("Upreeling", true);
        }
    }
}
