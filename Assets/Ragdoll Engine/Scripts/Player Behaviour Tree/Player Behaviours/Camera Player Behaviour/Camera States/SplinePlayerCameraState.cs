using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine
{
    public class SplinePlayerCameraState : PlayerCameraState
    {
        Volume volume;

        SplineContainer projectedSplineContainer;

        SplineContainer cameraSplineContainer;

        Vector3 rotation;

        bool spline;

        bool followPlayer;

        float clampMin;

        float clampMax;

        public override void Execute()
        {
            spline = SplineCameraCheck();

            if (!spline) return;

            //cameraTransform.position = Vector3.Lerp(oldPosition + playerTransform.position,
            //ProjectSpline(),
            //Mathf.SmoothStep(1, 0, transition / transitionTime));

            cameraTransform.position = modelTransform.position
                + Vector3.Slerp(oldPosition,
                    ProjectSpline() - modelTransform.position,
                    Mathf.SmoothStep(1, 0, transition / transitionTime));

            cameraTransform.rotation = Quaternion.Slerp(oldRotation,
                Quaternion.Euler(rotation)
                    * (followPlayer ? Quaternion.LookRotation(modelTransform.position - cameraTransform.position, Vector3.up)
                        : Quaternion.identity),
                1 - Mathf.Sin(Mathf.SmoothStep(0, 1, transition / transitionTime) * Mathf.PI / 2));
        }

        public override bool Check()
        {
            return SplineCameraCheck();
        }

        bool SplineCameraCheck()
        {
            if (spline)
                foreach (Volume thisVolume in volumes)
                    if (thisVolume is SplineCameraVolume && thisVolume == volume)
                        return true;

            foreach (Volume thisVolume in volumes)
                if (thisVolume is SplineCameraVolume)
                {
                    volume = thisVolume;

                    SplineCameraVolume thisSplineCameraVolume = thisVolume as SplineCameraVolume;

                    projectedSplineContainer = thisSplineCameraVolume.projectedSplineContainer;

                    cameraSplineContainer = thisSplineCameraVolume.cameraSplineContainer;

                    rotation = thisSplineCameraVolume.rotation;

                    followPlayer = thisSplineCameraVolume.followPlayer;

                    clampMin = thisSplineCameraVolume.clampMin;

                    clampMax = thisSplineCameraVolume.clampMax;

                    return true;
                }

            return false;
        }

        Vector3 ProjectSpline()
        {
            SplineUtility.GetNearestPoint(projectedSplineContainer.Spline, Utility.DivideVector3(playerTransform.position - projectedSplineContainer.transform.position, projectedSplineContainer.transform.lossyScale), out float3 _, out float t);

            return cameraSplineContainer.EvaluatePosition(Mathf.Clamp01((t - clampMin) / clampMax));
        }
    }
}
