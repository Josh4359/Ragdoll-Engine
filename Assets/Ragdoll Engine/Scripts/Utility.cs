using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine
{
    public static class Utility
    {
        [Serializable]
        public struct TransformData
        {
            public Vector3 position;

            public Vector3 rotation;

            public Vector3 scale;
        }

        public static TransformData TransformToData(Transform transform)
        {
            return new TransformData
            {
                position = transform.position,
                rotation = transform.rotation.eulerAngles,
                scale = transform.localScale
            };
        }

        public static Vector3 DivideVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x,
                a.y / b.y,
                a.z / b.z);
        }

        // Extra spline functions
        struct Segment
        {
            public float start, length;

            public Segment(float start, float length)
            {
                this.start = start;
                this.length = length;
            }
        }

        public delegate bool PointParameter(float3 point);

        public static float GetNearestPoint<T>(T spline,
            float3 point,
            PointParameter pointParameter,
            out float3 nearest,
            out float t,
            out bool success,
            int resolution = SplineUtility.PickResolutionDefault,
            int iterations = 2) where T : ISpline
        {
            float distance = float.PositiveInfinity;
            nearest = float.PositiveInfinity;
            Segment segment = new Segment(0f, 1f);
            t = 0f;
            success = false;
            int res = math.min(math.max(SplineUtility.PickResolutionMin, resolution), SplineUtility.PickResolutionMax);

            for (int i = 0, c = math.min(10, iterations); i < c; i++)
            {
                int segments = SplineUtility.GetSubdivisionCount(spline.GetLength() * segment.length, res);
                segment = GetNearestPoint(spline, point, pointParameter, segment, out distance, out nearest, out t, out success, segments);
            }

            return distance;
        }

        static Segment GetNearestPoint<T>(T spline,
            float3 point,
            PointParameter pointParameter,
            Segment range,
            out float distance, out float3 nearest, out float time, out bool success,
            int segments) where T : ISpline
        {
            distance = float.PositiveInfinity;
            nearest = float.PositiveInfinity;
            time = float.PositiveInfinity;
            success = false;
            Segment segment = new Segment(-1f, 0f);

            float t0 = range.start;
            float3 a = SplineUtility.EvaluatePosition(spline, t0);

            for (int i = 1; i < segments; i++)
            {
                float t1 = range.start + (range.length * (i / (segments - 1f)));
                float3 b = SplineUtility.EvaluatePosition(spline, t1);
                var p = SplineMath.PointLineNearestPoint(point, a, b, out var lineParam);
                float dsqr = math.distancesq(p, point);

                if (dsqr < distance && pointParameter(p))
                {
                    segment.start = t0;
                    segment.length = t1 - t0;
                    time = segment.start + segment.length * lineParam;
                    distance = dsqr;

                    nearest = p;
                    success = true;
                }

                t0 = t1;
                a = b;
            }

            distance = math.sqrt(distance);
            return segment;
        }
    }
}
