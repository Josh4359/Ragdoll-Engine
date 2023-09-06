using UnityEngine;

namespace RagdollEngine
{
    public class GrindBoosterRailExtension : RailExtension
    {
        public override void Execute()
        {
            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is GrindBoosterStageObject)
                {
                    if (wasActive)
                    {
                        active = true;

                        return;
                    }

                    GrindBoosterStageObject thisGrindBoosterStageObject = thisStageObject as GrindBoosterStageObject;

                    thisGrindBoosterStageObject.audioSource.Play();

                    Vector3 direction = matrix.c2 * Mathf.Sign(Vector3.Dot(thisGrindBoosterStageObject.transform.forward, matrix.c2));

                    velocity = direction * Mathf.Max(thisGrindBoosterStageObject.speed * Time.fixedDeltaTime, Vector3.Dot(velocity, direction));

                    extend = false;

                    return;
                }
        }
    }
}
