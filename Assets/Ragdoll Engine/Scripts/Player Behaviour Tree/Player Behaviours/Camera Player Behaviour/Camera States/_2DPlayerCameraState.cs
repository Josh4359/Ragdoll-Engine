using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine
{
    public class _2DPlayerCameraState : PlayerCameraState
    {
        [SerializeField] _2DPlayerBehaviour _2DPlayerBehaviour;

        [SerializeField] Vector2 cameraOffset;

        [SerializeField] float slopeOffset;

        [SerializeField] float cameraSmoothness;

        [SerializeField] float cameraTangentDeadzone;

        [SerializeField] float rotationOffsetTransitionTime;

        [SerializeField, Tooltip("Y axis transition time after entering ground")] float yTransitionTime;

        Volume volume;

        SplineContainer splineContainer;

        Quaternion matrixRotation;

        Quaternion oldMatrixRotation;

        Quaternion rotationOffset;

        Quaternion currentRotationOffset;

        Quaternion oldRotationOffset;

        Vector3 currentCameraOffset;

        Vector3 smoothCameraOffset;

        float3x3 matrix;

        bool _2D;

        float depth;

        float cameraTangent;

        float rotationOffsetTransition;

        public override void Execute()
        {
            _2D = _2DCameraCheck();

            if (!_2D) return;

            if (respawnTrigger)
                transition = 0;

            matrix = EvaluateSpline();

            rotationOffsetTransition = Mathf.Max(rotationOffsetTransition - Time.fixedDeltaTime, 0);

            matrixRotation = Quaternion.Slerp(oldMatrixRotation, Quaternion.LookRotation(-matrix.c0, matrix.c1), Mathf.SmoothStep(1, 0, rotationOffsetTransition / rotationOffsetTransitionTime));

            currentRotationOffset = Quaternion.Slerp(oldRotationOffset, rotationOffset, Mathf.SmoothStep(1, 0, rotationOffsetTransition / rotationOffsetTransitionTime));

            Quaternion cameraGoalRotation = matrixRotation
                * currentRotationOffset;
                //Quaternion.Euler(Vector3.Lerp(oldRotationOffset, rotationOffset, Mathf.SmoothStep(1, 0, rotationOffsetTransition / rotationOffsetTransitionTime)));

            currentCameraOffset = CalculateCameraOffset(matrix);

            smoothCameraOffset = Vector3.Lerp(smoothCameraOffset, currentCameraOffset, 1 - cameraSmoothness);

            Vector3 cameraGoalPosition = modelTransform.position + (cameraTransform.right * smoothCameraOffset.x) + (cameraTransform.up * smoothCameraOffset.y) + (cameraTransform.forward * smoothCameraOffset.z);

            cameraTransform.position = modelTransform.position
                + Vector3.Slerp(oldPosition,
                    cameraGoalPosition - modelTransform.position,
                    Mathf.SmoothStep(1, 0, transition / transitionTime));

            cameraTransform.rotation = Quaternion.Slerp(oldRotation,
                cameraGoalRotation,
                1 - Mathf.Sin(Mathf.SmoothStep(0, 1, transition / transitionTime) * Mathf.PI / 2));

            base.Execute();
        }

        public override bool Check()
        {
            return _2DCameraCheck();
        }

        public override void Enable()
        {
            matrix = EvaluateSpline();

            cameraTangent = 0;

            currentCameraOffset = Vector3.zero;

            smoothCameraOffset = Vector3.zero;

            rotationOffsetTransition = 0;
        }

        public override void Transition()
        {
            base.Transition();

            Enable();
        }

        Vector3 CalculateCameraOffset(float3x3 currentMatrix)
        {
            float tangentDot = Vector3.Dot(moveVelocity, Vector3.Cross(currentMatrix.c0, Vector3.up));

            float upDot = Vector3.Dot(playerTransform.up, Vector3.up);

            if (tangentDot > cameraTangentDeadzone)
                cameraTangent = Mathf.Abs(upDot);
            else if (tangentDot < -cameraTangentDeadzone)
                cameraTangent = -Mathf.Abs(upDot);

            return Vector3.Scale(cameraOffset, new Vector3(
                        cameraTangent,
                        groundInformation.ground ? upDot : (currentCameraOffset.y / cameraOffset.y)
                    )
                    + Vector3.right * Vector3.Dot(playerTransform.up, cameraTransform.right) * slopeOffset)
                    + -Vector3.forward * depth;
        }

        float3x3 EvaluateSpline()
        {
            SplineUtility.GetNearestPoint(splineContainer.Spline, Utility.DivideVector3(playerTransform.position - splineContainer.transform.position, splineContainer.transform.lossyScale), out float3 _, out float t);

            float3x3 matrix;

            splineContainer.Evaluate(t, out float3 _, out matrix.c2, out matrix.c1);

            return new float3x3()
            {
                c0 = Vector3.Cross(matrix.c1, matrix.c2).normalized,
                c1 = Vector3.Normalize(matrix.c1),
                c2 = Vector3.Normalize(matrix.c2)
            };
        }

        bool _2DCameraCheck()
        {
            if (_2D)
                foreach (Volume thisVolume in volumes)
                    if (thisVolume is _2DCameraVolume && thisVolume == volume)
                        return true;

            foreach (Volume thisVolume in volumes)
                if (thisVolume is _2DCameraVolume)
                {
                    _2DCameraVolume this2DCameraVolume = thisVolume as _2DCameraVolume;

                    splineContainer = this2DCameraVolume.splineContainer;

                    oldMatrixRotation = matrixRotation;

                    oldRotationOffset = currentRotationOffset;

                    rotationOffsetTransition = rotationOffsetTransitionTime;

                    rotationOffset = Quaternion.Euler(this2DCameraVolume.rotationOffset);

                    depth = this2DCameraVolume.depth;

                    volume = thisVolume;

                    return true;
                }

            return false;
        }
    }
}
