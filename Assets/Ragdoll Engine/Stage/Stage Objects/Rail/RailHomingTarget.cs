using Unity.Mathematics;
using UnityEngine;

namespace RagdollEngine
{
    [RequireComponent(typeof(RailStageObject))]
    public class RailHomingTarget : HomingTarget
    {
        RailStageObject railStageObject;

        void Awake()
        {
            railStageObject = GetComponent<RailStageObject>();
        }

        public override bool Target(PlayerBehaviourTree playerBehaviourTree, float maxDistance, float viewDot, float maxHeight, out Vector3 point)
        {
            if (!base.Target(playerBehaviourTree, maxDistance, viewDot, maxHeight, out point)) return false;

            RailPlayerBehaviour railPlayerBehaviour = playerBehaviourTree.GetComponentInChildren<RailPlayerBehaviour>();

            if (!railPlayerBehaviour || railPlayerBehaviour.rail) return false;

            float3 point1 = Utility.DivideVector3(playerBehaviourTree.playerTransform.position - railStageObject.splineContainer.transform.position, railStageObject.splineContainer.transform.lossyScale);

            Utility.GetNearestPoint(railStageObject.splineContainer.Spline,
                point1,
                p =>
                {
                    return Vector3.Dot(Vector3.Normalize(p - point1), playerBehaviourTree.modelTransform.forward) > viewDot
                    && (p.y - point1.y) / maxDistance < maxHeight;
                },
                out _, out float t, out bool success);

            if (!success) return false;

            railStageObject.splineContainer.Evaluate(t, out float3 nearest, out _, out float3 up);

            point = nearest + (up * playerBehaviourTree.height);

            return true;
        }

        public override void OnTarget(PlayerBehaviourTree playerBehaviourTree)
        {
            RailPlayerBehaviour railPlayerBehaviour = playerBehaviourTree.GetComponentInChildren<RailPlayerBehaviour>();

            if (!railPlayerBehaviour) return;

            railPlayerBehaviour.Enter(railStageObject);
        }
    }
}
