using UnityEngine;

namespace RagdollEngine
{
    [RequireComponent(typeof(UpreelStageObject))]
    public class UpreelHomingTarget : HomingTarget
    {
        UpreelStageObject upreelStageObject;

        void Awake()
        {
            upreelStageObject = GetComponent<UpreelStageObject>();
        }

        public override void OnTarget(PlayerBehaviourTree playerBehaviourTree)
        {
            UpreelPlayerBehaviour upreelPlayerBehaviour = playerBehaviourTree.GetComponentInChildren<UpreelPlayerBehaviour>();

            if (!upreelPlayerBehaviour) return;

            upreelPlayerBehaviour.Enter(upreelStageObject);
        }
    }
}
