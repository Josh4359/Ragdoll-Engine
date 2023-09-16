using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine
{
    public class SwitchRailExtension : RailExtension
    {
        [SerializeField] float switchDistance;

        [SerializeField] float switchTransitionTime;

        [SerializeField] float switchTransitionHeight;

        [SerializeField] float reverseCoyoteTime;

        Vector3 difference;

        float currentSwitchDirection;

        float switchTransition;

        float currentReverseCoyoteTime;

        public override void Execute()
        {
            if (inputHandler.sidestep.pressed)
                currentSwitchDirection = Mathf.Sign(inputHandler.sidestep.value * Vector3.Dot(cameraTransform.forward, velocity));

            Vector3 rayNormal = matrix.c0 * currentSwitchDirection * Mathf.Sign(Vector3.Dot(velocity, matrix.c2));

            if (switchTransition <= 0
                    && Physics.SphereCast(point + rayNormal, 0.5f, rayNormal, out RaycastHit hit, switchDistance - 0.5f, railLayerMask, QueryTriggerInteraction.Ignore)
                    && (inputHandler.sidestep.pressed
                        || currentReverseCoyoteTime > 0))
            {
                hit.collider.TryGetComponent(out RailStageObject railStageObject);

                if (!railStageObject) return;

                currentReverseCoyoteTime = 0;

                SplineUtility.GetNearestPoint(railStageObject.splineContainer.Spline, Utility.DivideVector3(hit.point - railStageObject.splineContainer.transform.position, railStageObject.splineContainer.transform.lossyScale), out float3 _, out float currentT);

                t = currentT;

                float3x3 currentMatrix;

                railStageObject.splineContainer.Evaluate(t, out float3 nearest, out currentMatrix.c2, out currentMatrix.c1);

                currentMatrix.c2 = Vector3.Normalize(currentMatrix.c2);

                currentMatrix.c1 = Vector3.Normalize(currentMatrix.c1);

                currentMatrix.c0 = Vector3.Cross(currentMatrix.c1, currentMatrix.c2);

                splineContainer = railStageObject.splineContainer;

                velocity = Vector3.Project(velocity, currentMatrix.c2);

                //switchDirection = currentSwitchDirection * Mathf.Sign(Vector3.Dot(velocity, matrix.c2) * Vector3.Dot(currentMatrix.c2, matrix.c2));

                matrix = currentMatrix;

                switchTransition = switchTransitionTime;

                //switchTransitionDistance = Vector3.Distance(nearest, point);

                point = nearest;

                goal = nearest + (matrix.c1 * height);

                difference = playerTransform.position - goal;

                animator.SetTrigger("Switch");

                animator.SetFloat("Switch Direction", currentSwitchDirection);
            }
            else if (inputHandler.sidestep.pressed)
                currentReverseCoyoteTime = reverseCoyoteTime;
            else
                currentReverseCoyoteTime = Mathf.Max(currentReverseCoyoteTime - Time.fixedDeltaTime, 0);

            if (switchTransition > 0)
            {
                goal += Switch();

                switchTransition = Mathf.Max(switchTransition - Time.fixedDeltaTime, 0);

                onRail = switchTransition <= 0;

                extend = false;
            }

            Vector3 Switch()
            {
                return (difference * (switchTransition / switchTransitionTime))
                    + (Vector3)(matrix.c1 * switchTransitionHeight * Mathf.Sin((1 - (switchTransition / switchTransitionTime)) * Mathf.PI));
            }
        }

        public override void Enable()
        {
            switchTransition = 0;

            currentReverseCoyoteTime = 0;
        }
    }
}
