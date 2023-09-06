using UnityEngine;

namespace RagdollEngine
{
    public class BoostCapsuleCollector : Collector
    {
        [SerializeField] BoostPlayerBehaviour boostPlayerBehaviour;

        public override void Execute()
        {
            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is BoostCapsuleCollectible)
                {
                    BoostCapsuleCollectible thisBoostCapsuleCollectible = thisStageObject as BoostCapsuleCollectible;

                    if (thisBoostCapsuleCollectible.collected) continue;

                    thisBoostCapsuleCollectible.Collect();

                    boostPlayerBehaviour.boostPercent = 1;
                }
        }
    }
}
