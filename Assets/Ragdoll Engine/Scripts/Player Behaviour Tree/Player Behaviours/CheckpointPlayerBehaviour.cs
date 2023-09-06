using System;
using UnityEngine;

namespace RagdollEngine
{
    public class CheckpointPlayerBehaviour : PlayerBehaviour
    {
        public override void Execute()
        {
            foreach (StageObject thisStageObject in stageObjects)
                if (thisStageObject is CheckpointStageObject)
                {
                    CheckpointStageObject thisCheckpointStageObject = thisStageObject as CheckpointStageObject;

                    if (thisCheckpointStageObject.characters.Contains(character)) return;

                    thisCheckpointStageObject.Checkpoint(this);

                    character.respawnTransformData.position = thisCheckpointStageObject.checkpointTransform.position;

                    character.respawnTransformData.rotation = thisCheckpointStageObject.checkpointTransform.rotation.eulerAngles;
                }
        }
    }
}
