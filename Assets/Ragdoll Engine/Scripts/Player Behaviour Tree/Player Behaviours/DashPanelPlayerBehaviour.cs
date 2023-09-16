using UnityEngine;

namespace RagdollEngine
{
    public class DashPanelPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float cooldownTime;

        DashPanelStageObject dashPanelStageObject;

        float cooldownTimer;

        public override bool Evaluate()
        {
            bool dashPanel = DashPanelCheck();

            if (dashPanel)
                additiveVelocity = -RB.velocity
                        + (dashPanelStageObject.transform.forward * Mathf.Max(dashPanelStageObject.speed, Vector3.Dot(RB.velocity, dashPanelStageObject.transform.forward)));

            return dashPanel;
        }

        bool DashPanelCheck()
        {
            if (!wasActive)
                cooldownTimer = 0;

            if (!groundInformation.ground) return false;

            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is DashPanelStageObject)
                {
                    cooldownTimer = cooldownTime;

                    if (wasActive) return true;

                    dashPanelStageObject = thisStageObject as DashPanelStageObject;

                    dashPanelStageObject.audioSource.Play();

                    return true;
                }

            cooldownTimer -= Time.fixedDeltaTime;

            if (cooldownTimer > 0) return true;

            return false;
        }
    }
}
