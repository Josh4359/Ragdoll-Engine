using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine
{
    public class RailPlayerBehaviour : PlayerBehaviour
    {
        public LayerMask railLayerMask;

        [SerializeField] RailExtension[] preExtensions;

        [SerializeField] RailExtension[] postExtensions;

        [SerializeField] float distance;

        [SerializeField] float minSpeed;

        [SerializeField] float uphillSlopeRatio;

        [SerializeField] float downhillSlopeRatio;

        [HideInInspector] public SplineContainer splineContainer;

        [HideInInspector] public float3x3 matrix;

        [HideInInspector] public Vector3 point;

        [HideInInspector] public Vector3 goal;

        [HideInInspector] public Vector3 velocity;

        [HideInInspector] public bool rail;

        [HideInInspector] public bool pass;

        [HideInInspector] public bool onRail;

        [HideInInspector] public bool extend;

        [HideInInspector] public float t;

        bool enter;

        void LateUpdate()
        {
            animator.SetBool("Grinding", active);
        }

        public override bool Evaluate()
        {
            return pass;
        }

        public override void Execute()
        {
            pass = false;

            if (enter)
                enter = false;
            else if (!wasActive)
                rail = false;

            if (!rail)
            {
                bool cast = Physics.Raycast(playerTransform.position, -playerTransform.up, out RaycastHit hit, distance + (Mathf.Max(Vector3.Dot(RB.velocity, -playerTransform.up), 0) * Time.fixedDeltaTime), railLayerMask, QueryTriggerInteraction.Ignore);

                if (!cast) return;

                hit.collider.TryGetComponent(out RailStageObject railStageObject);

                if (!railStageObject) return;

                SplineUtility.GetNearestPoint(railStageObject.splineContainer.Spline, Utility.DivideVector3(hit.point - railStageObject.splineContainer.transform.position, railStageObject.splineContainer.transform.lossyScale), out float3 _, out t);

                Vector3 pointTangent = ((Vector3)railStageObject.splineContainer.EvaluateTangent(t)).normalized;

                railStageObject.splineContainer.Evaluate(t, out float3 nearest, out matrix.c2, out matrix.c1);

                matrix.c2 = Vector3.Normalize(matrix.c2);

                matrix.c1 = Vector3.Normalize(matrix.c1);

                matrix.c0 = Vector3.Cross(matrix.c1, matrix.c2);

                point = nearest;

                goal = nearest + (matrix.c1 * height);

                Vector3 difference = goal - playerTransform.position;

                Vector3 difference1 = point - playerTransform.position;

                if (Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore)
                    || Physics.Raycast(playerTransform.position, difference1.normalized, difference1.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                    return;

                float t1 = t + (RB.velocity.magnitude * Mathf.Sign(Vector3.Dot(RB.velocity, pointTangent)) * Time.fixedDeltaTime / railStageObject.splineContainer.CalculateLength());

                railStageObject.splineContainer.Evaluate(t1, out float3 nearest1, out _, out _);

                float length = railStageObject.splineContainer.CalculateLength();

                rail = Vector3.Dot(RB.velocity, (Vector3)nearest1 - hit.point) > 0
                    && (groundInformation.ground || Vector3.Dot(RB.velocity, matrix.c1) <= 0)
                    && (!groundInformation.ground || hit.distance < groundInformation.hit.distance)
                    && (t1 * length > 1 || Vector3.Dot(RB.velocity, matrix.c2) > 0)
                    && (length - (t1 * length) > 1 || Vector3.Dot(RB.velocity, -matrix.c2) > 0);

                if (!rail) return;

                onRail = true;

                foreach (RailExtension thisRailExtension in preExtensions)
                    thisRailExtension.Enable();

                foreach (RailExtension thisRailExtension in postExtensions)
                    thisRailExtension.Enable();

                splineContainer = railStageObject.splineContainer;

                velocity = matrix.c2 * Mathf.Sign(Vector3.Dot(RB.velocity, matrix.c2)) * RB.velocity.magnitude * Time.fixedDeltaTime;
            }

            if (rail)
            {
                splineContainer.Evaluate(t, out float3 nearest, out matrix.c2, out matrix.c1);

                matrix.c1 = Vector3.Normalize(matrix.c1);

                matrix.c2 = Vector3.Normalize(matrix.c2);

                matrix.c0 = Vector3.Cross(matrix.c1, matrix.c2);

                float dot = Vector3.Dot(velocity, matrix.c2)
                    + (Vector3.Dot(matrix.c2 * Mathf.Sign(Vector3.Dot(velocity, matrix.c2)), -Vector3.up)
                        * Mathf.Sign(Vector3.Dot(velocity, matrix.c2))
                        * (Vector3.Dot(matrix.c2 * Vector3.Dot(velocity, matrix.c2), Vector3.up) > 0 ? uphillSlopeRatio : downhillSlopeRatio)
                        * Time.fixedDeltaTime);

                velocity = matrix.c2 * dot;

                ExecuteExtensions(preExtensions, out rail);

                if (!rail) return;

                t += dot / splineContainer.CalculateLength();

                point = nearest;

                goal = nearest + (matrix.c1 * height);

                extend = true;

                ExecuteExtensions(postExtensions, out rail);

                if (!rail) return;

                Vector3 difference = goal - playerTransform.position;

                Vector3 difference1 = point - playerTransform.position;

                if (Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore)
                    || Physics.Raycast(playerTransform.position, difference1.normalized, difference1.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                {
                    rail = false;

                    return;
                }

                movePosition = goal;

                overrideModelTransform = true;

                modelTransform.position = goal - (Vector3)(matrix.c1 * height);

                modelTransform.rotation = Quaternion.LookRotation(matrix.c2 * Mathf.Sign(dot), matrix.c1);

                kinematic = true;

                pass = true;

                if ((dot >= 0 && t > 1)
                    || (dot <= 0 && t < 0))
                {
                    rail = false;

                    return;
                }

                if (!wasActive)
                    animator.SetTrigger("Grind");
            }
            else if (wasActive)
                rail = false;
        }

        public void Enter(RailStageObject railStageObject)
        {
            SplineUtility.GetNearestPoint(railStageObject.splineContainer.Spline, Utility.DivideVector3(playerTransform.position - railStageObject.splineContainer.transform.position, railStageObject.splineContainer.transform.lossyScale), out float3 _, out t);

            splineContainer = railStageObject.splineContainer;

            splineContainer.Evaluate(t, out _, out matrix.c2, out matrix.c1);

            matrix.c2 = Vector3.Normalize(matrix.c2);

            matrix.c1 = Vector3.Normalize(matrix.c1);

            matrix.c0 = Vector3.Cross(matrix.c1, matrix.c2);

            velocity = matrix.c2 * Mathf.Sign(Vector3.Dot(RB.velocity, matrix.c2)) * RB.velocity.magnitude * Time.fixedDeltaTime;

            active = true;

            rail = true;

            pass = true;

            onRail = true;

            enter = true;

            foreach (RailExtension thisRailExtension in preExtensions)
                thisRailExtension.Enable();

            foreach (RailExtension thisRailExtension in postExtensions)
                thisRailExtension.Enable();

            animator.SetTrigger("Grind");

            kinematic = true;
        }

        void ExecuteExtensions(RailExtension[] railExtensions, out bool successful)
        {
            foreach (RailExtension thisRailExtention in railExtensions)
            {
                thisRailExtention.Execute();

                if (!rail)
                {
                    successful = false;

                    return;
                }

                if (!extend)
                {
                    successful = true;

                    return;
                }
            }

            successful = true;
        }
    }

    public class RailExtension : PlayerBehaviour
    {
        public RailPlayerBehaviour railPlayerBehaviour
        {
            get
            {
                return GetComponentInParent<RailPlayerBehaviour>();
            }
        }

        public LayerMask railLayerMask
        {
            get
            {
                return railPlayerBehaviour.railLayerMask;
            }
        }

        public SplineContainer splineContainer
        {
            get
            {
                return railPlayerBehaviour.splineContainer;
            }
            set
            {
                railPlayerBehaviour.splineContainer = value;
            }
        }

        public float3x3 matrix
        {
            get
            {
                return railPlayerBehaviour.matrix;
            }
            set
            {
                railPlayerBehaviour.matrix = value;
            }
        }

        public Vector3 point
        {
            get
            {
                return railPlayerBehaviour.point;
            }
            set
            {
                railPlayerBehaviour.point = value;
            }
        }

        public Vector3 goal
        {
            get
            {
                return railPlayerBehaviour.goal;
            }
            set
            {
                railPlayerBehaviour.goal = value;
            }
        }

        public Vector3 velocity
        {
            get
            {
                return railPlayerBehaviour.velocity;
            }
            set
            {
                railPlayerBehaviour.velocity = value;
            }
        }

        public bool rail
        {
            get
            {
                return railPlayerBehaviour.rail;
            }
            set
            {
                railPlayerBehaviour.rail = value;
            }
        }

        public bool pass
        {
            get
            {
                return railPlayerBehaviour.pass;
            }
            set
            {
                railPlayerBehaviour.pass = value;
            }
        }

        public bool onRail
        {
            get
            {
                return railPlayerBehaviour.onRail;
            }
            set
            {
                railPlayerBehaviour.onRail = value;
            }
        }

        public bool extend
        {
            get
            {
                return railPlayerBehaviour.extend;
            }
            set
            {
                railPlayerBehaviour.extend = value;
            }
        }

        public float t
        {
            get
            {
                return railPlayerBehaviour.t;
            }
            set
            {
                railPlayerBehaviour.t = value;
            }
        }

        public virtual void Enable() { }
    }
}
