using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine
{
    public class _2DPlayerBehaviour : PlayerBehaviour
    {
        [HideInInspector] public Vector3 position;

        [HideInInspector] public float3x3 matrix;

        [HideInInspector] public bool _2D;

        [HideInInspector] public bool air;

        Volume volume;

        SplineContainer splineContainer;

        SplineContainer airSplineContainer;

        bool loop;

        public override void Execute()
        {
            if (_2DCheck())
            {
                air = _2D && loop && !groundInformation.ground;

                _2D = true;
            }
            else
            {
                _2D = false;

                air = false;

                tangent = Vector3.zero;

                plane = Vector3.zero;

                return;
            }

            SplineContainer thisSplineContainer = air ? airSplineContainer : splineContainer;

            SplineUtility.GetNearestPoint(thisSplineContainer.Spline, Utility.DivideVector3(playerTransform.position - thisSplineContainer.transform.position, thisSplineContainer.transform.lossyScale), out float3 _, out float t);

            thisSplineContainer.Evaluate(t, out float3 nearest, out matrix.c2, out matrix.c1);

            matrix.c1 = Vector3.Normalize(matrix.c1);

            matrix.c2 = Vector3.Normalize(matrix.c2);

            matrix.c0 = Vector3.Cross(matrix.c1, matrix.c2).normalized;

            tangent = matrix.c2;

            plane = matrix.c0;

            position = air ? (Vector3)nearest + Vector3.Project(position - (Vector3)nearest, plane) : nearest;
        }

        bool _2DCheck()
        {
            foreach (Volume thisVolume in volumes)
                if (thisVolume is LoopVolume)
                {
                    volume = thisVolume;

                    splineContainer = ((LoopVolume)thisVolume).splineContainer;

                    airSplineContainer = ((LoopVolume)thisVolume).airSplineContainer;

                    loop = true;

                    return true;
                }

            loop = false;

            if (_2D)
                foreach (Volume thisVolume in volumes)
                    if (thisVolume is SplineVolume && thisVolume == volume)
                        return true;

            foreach (Volume thisVolume in volumes)
                if (thisVolume is SplineVolume)
                {
                    volume = thisVolume;

                    splineContainer = ((SplineVolume)thisVolume).splineContainer;

                    return true;
                }

            return false;
        }
    }
}
