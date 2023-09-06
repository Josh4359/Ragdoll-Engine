using Unity.Mathematics;
using UnityEngine;

namespace RagdollEngine
{
    public class _2DSnapPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] _2DPlayerBehaviour _2DPlayerBehaviour;

        [SerializeField] float transitionTime;

        Vector3 oldPosition;

        float transition;

        Vector3 position => _2DPlayerBehaviour.position;

        float3x3 matrix => _2DPlayerBehaviour.matrix;

        bool _2D => _2DPlayerBehaviour._2D;

        bool air => _2DPlayerBehaviour.air;

        bool wasAir;

        public override void Execute()
        {
            if (!_2D)
            {
                oldPosition = playerTransform.position;

                transition = transitionTime;

                wasAir = false;

                return;
            }

            if (respawnTrigger)
                transition = 0;

            if (kinematic) return;

            if (!air && wasAir)
            {
                oldPosition = playerTransform.position;

                transition = transitionTime;
            }

            float t = Mathf.Max(transition - (RB.velocity.magnitude * Time.fixedDeltaTime * Time.fixedDeltaTime), 0);

            Vector3 goal = Vector3.Lerp(playerTransform.position - Vector3.Project(playerTransform.position - oldPosition, matrix.c0),
                playerTransform.position - Vector3.Project(playerTransform.position - position, matrix.c0),
                1 - (t / transitionTime));

            Vector3 difference = goal - playerTransform.position;

            if (!Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore))
            {
                playerTransform.position = goal;

                transition = t;
            }

            wasAir = air;
        }
    }
}
