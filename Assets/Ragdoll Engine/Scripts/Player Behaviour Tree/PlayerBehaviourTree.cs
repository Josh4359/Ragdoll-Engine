using BehaviourGraph;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RagdollEngine
{
    using static PlayerBehaviourTree;

#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerBehaviourTree))]
    class PlayerBehaviourTreeEditor : BehaviourTreeEditor { }

#endif

    public class PlayerBehaviourTree : BehaviourTree
    {
        public Transform playerTransform;

        public Transform modelTransform;

        public Rigidbody RB;

        public Animator animator;

        public GroundInformation groundInformation;

        public LayerMask layerMask;

        public float height;

        public float moveDeadzone;

        [SerializeField, Tooltip("Dynamically increase collision accuracy")] bool dynamicSolverIterations;

        [SerializeField] float solverIterationDistance;

        [HideInInspector] public List<Volume> volumes;

        [HideInInspector] public List<StageObject> stageObjects;

        [HideInInspector] public Character character;

        public Transform cameraTransform => character.cameraTransform;

        public Canvas canvas => character.canvas;

        public InputHandler inputHandler => character.inputHandler;

        [HideInInspector] public Vector3 additiveVelocity;

        [HideInInspector] public Vector3 movePosition;

        [HideInInspector] public Vector3 moveVelocity;

        [HideInInspector] public Vector3 accelerationVector;

        [HideInInspector] public Vector3 tangent;

        [HideInInspector] public Vector3 plane;

        [HideInInspector] public bool overrideModelTransform;

        [HideInInspector] public bool kinematic;

        [HideInInspector] public bool respawnTrigger;

        [HideInInspector] public bool moving;

        [HideInInspector] public bool initialized;

        public struct GroundInformation
        {
            public RaycastHit hit;

            public bool ground;

            public bool cast;

            public bool slope;

            public bool enter;
        }

        public override void FixedUpdate()
        {
            if (!initialized)
            {
                RB.isKinematic = true;

                return;
            }

            additiveVelocity = Vector3.zero;

            movePosition = RB.position;

            moveVelocity = Vector3.ProjectOnPlane(RB.velocity, playerTransform.up);

            accelerationVector = Vector3.zero;

            overrideModelTransform = false;

            kinematic = false;

            respawnTrigger = false;

            moving = false;

            RB.isKinematic = false;

            base.FixedUpdate();

            Cursor.lockState = CursorLockMode.Locked;

            RB.isKinematic = kinematic;

            if (kinematic)
                RB.MovePosition(movePosition);
            else
            {
                additiveVelocity += accelerationVector;

                RB.velocity += additiveVelocity;

                RB.velocity -= Vector3.Project(RB.velocity, plane);

                if (groundInformation.ground)
                    RB.velocity -= Vector3.Project(RB.velocity, groundInformation.hit.normal);
            }

            if (dynamicSolverIterations)
                RB.solverIterations = Mathf.Max(Mathf.FloorToInt(RB.velocity.magnitude * Time.fixedDeltaTime / solverIterationDistance), Physics.defaultSolverIterations);
            else
                RB.solverIterations = Physics.defaultSolverIterations;
        }

        public void Initialize(Character character)
        {
            this.character = character;

            initialized = true;
        }
    }

    public class PlayerBehaviour : BehaviourGraph.Behaviour
    {
        public PlayerBehaviourTree playerBehaviourTree => behaviourTree as PlayerBehaviourTree;

        public Transform playerTransform => playerBehaviourTree.playerTransform;

        public Transform modelTransform => playerBehaviourTree.modelTransform;

        public Rigidbody RB => playerBehaviourTree.RB;

        public Animator animator => playerBehaviourTree.animator;

        public LayerMask layerMask => playerBehaviourTree.layerMask;

        public float height => playerBehaviourTree.height;

        public float moveDeadzone => playerBehaviourTree.moveDeadzone;

        public GroundInformation groundInformation
        {
            get => playerBehaviourTree.groundInformation;
            set => playerBehaviourTree.groundInformation = value;
        }

        public List<Volume> volumes
        {
            get => playerBehaviourTree.volumes;
            set => playerBehaviourTree.volumes = value;
        }

        public List<StageObject> stageObjects
        {
            get => playerBehaviourTree.stageObjects;
            set => playerBehaviourTree.stageObjects = value;
        }

        public Character character => playerBehaviourTree.character;

        public Transform cameraTransform => playerBehaviourTree.cameraTransform;

        public Canvas canvas => playerBehaviourTree.canvas;

        public InputHandler inputHandler => playerBehaviourTree.inputHandler;

        public Vector3 additiveVelocity
        {
            get => playerBehaviourTree.additiveVelocity;
            set => playerBehaviourTree.additiveVelocity = value;
        }

        public Vector3 movePosition
        {
            get => playerBehaviourTree.movePosition;
            set => playerBehaviourTree.movePosition = value;
        }

        public Vector3 moveVelocity
        {
            get => playerBehaviourTree.moveVelocity;
            set => playerBehaviourTree.moveVelocity = value;
        }

        public Vector3 accelerationVector
        {
            get => playerBehaviourTree.accelerationVector;
            set => playerBehaviourTree.accelerationVector = value;
        }

        public Vector3 tangent
        {
            get => playerBehaviourTree.tangent;
            set => playerBehaviourTree.tangent = value;
        }

        public Vector3 plane
        {
            get => playerBehaviourTree.plane;
            set => playerBehaviourTree.plane = value;
        }

        public bool overrideModelTransform
        {
            get => playerBehaviourTree.overrideModelTransform;
            set => playerBehaviourTree.overrideModelTransform = value;
        }

        public bool kinematic
        {
            get => playerBehaviourTree.kinematic;
            set => playerBehaviourTree.kinematic = value;
        }

        public bool respawnTrigger
        {
            get => playerBehaviourTree.respawnTrigger;
            set => playerBehaviourTree.respawnTrigger = value;
        }

        public bool moving
        {
            get => playerBehaviourTree.moving;
            set => playerBehaviourTree.moving = value;
        }
    }
}
