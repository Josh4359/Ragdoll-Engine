using UnityEngine;

namespace RagdollEngine
{
    public class DashPanelPlayerBehaviour : PlayerBehaviour
    {
        public override bool Evaluate()
        {
            if (!groundInformation.ground) return false;

            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is DashPanelStageObject)
                {
                    if (wasActive) return true;

                    DashPanelStageObject thisDashPanelStageObject = thisStageObject as DashPanelStageObject;

                    thisDashPanelStageObject.audioSource.Play();

                    additiveVelocity = -RB.velocity
                        + (thisStageObject.transform.forward * Mathf.Max(thisDashPanelStageObject.speed, Vector3.Dot(RB.velocity, thisDashPanelStageObject.transform.forward)));

                    return true;
                }

            return false;
        }
    }
}
