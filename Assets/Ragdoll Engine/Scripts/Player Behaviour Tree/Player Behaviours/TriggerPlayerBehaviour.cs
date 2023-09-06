using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RagdollEngine
{
    public class TriggerPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] LayerMask triggerLayerMask;

        [SerializeField] float minOverlapDistance;

        public override void Execute()
        {
            Collider[] theseColliders = new Collider[0];

            Vector3 oldPosition = playerTransform.position - (RB.velocity * Time.fixedDeltaTime);

            int resolution = Mathf.Max(Mathf.FloorToInt(RB.velocity.magnitude * Time.fixedDeltaTime / minOverlapDistance), 1);

            for (int i = 1; i < resolution + 1; i++)
            {
                Vector3 thisPosition = Vector3.Lerp(oldPosition, playerTransform.position, (float)i / resolution);

                foreach (Collider thisCollider in Physics.OverlapCapsule(thisPosition, thisPosition - (playerTransform.up * (groundInformation.cast ? groundInformation.hit.distance : height)), 0.5f, triggerLayerMask, QueryTriggerInteraction.Collide))
                    if (!theseColliders.Contains(thisCollider))
                        theseColliders = theseColliders.Append(thisCollider).ToArray();
            }

            stageObjects = new List<StageObject>();

            List<Volume> removedVolumes = new List<Volume>();

            foreach (Volume thisVolume in volumes)
            {
                bool inVolume = false;

                foreach (Trigger thisTrigger in thisVolume.triggers)
                {
                    if (theseColliders.Contains(thisTrigger.GetComponent<Collider>()))
                    {
                        inVolume = true;

                        break;
                    }
                }

                if (!inVolume)
                    removedVolumes.Add(thisVolume);
            }

            foreach (Volume thisVolume in removedVolumes)
                volumes.Remove(thisVolume);

            foreach (Collider thisCollider in theseColliders)
            {
                thisCollider.TryGetComponent(out Trigger thisTrigger);

                if (thisTrigger)
                    foreach (Volume thisVolume in thisTrigger.volumes)
                        if (!volumes.Contains(thisVolume))
                            volumes.Add(thisVolume);

                foreach (StageObject thisStageObject in thisCollider.GetComponents<StageObject>())
                    stageObjects.Add(thisStageObject);
            }
        }
    }
}
