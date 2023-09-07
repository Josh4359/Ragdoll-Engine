using BehaviourGraph;
using System;
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

        bool initialized;

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
        public PlayerBehaviourTree playerBehaviourTree
        {
            get
            {
                return behaviourTree as PlayerBehaviourTree;
            }
        }

        public Transform playerTransform
        {
            get
            {
                return playerBehaviourTree.playerTransform;
            }
        }

        public Transform modelTransform
        {
            get
            {
                return playerBehaviourTree.modelTransform;
            }
        }

        public Rigidbody RB
        {
            get
            {
                return playerBehaviourTree.RB;
            }
        }

        public Animator animator
        {
            get
            {
                return playerBehaviourTree.animator;
            }
        }

        public LayerMask layerMask
        {
            get
            {
                return playerBehaviourTree.layerMask;
            }
        }

        public float height
        {
            get
            {
                return playerBehaviourTree.height;
            }
        }

        public float moveDeadzone
        {
            get
            {
                return playerBehaviourTree.moveDeadzone;
            }
        }

        public GroundInformation groundInformation
        {
            get
            {
                return playerBehaviourTree.groundInformation;
            }
            set
            {
                playerBehaviourTree.groundInformation = value;
            }
        }

        public List<Volume> volumes
        {
            get
            {
                return playerBehaviourTree.volumes;
            }
            set
            {
                playerBehaviourTree.volumes = value;
            }
        }

        public List<StageObject> stageObjects
        {
            get
            {
                return playerBehaviourTree.stageObjects;
            }
            set
            {
                playerBehaviourTree.stageObjects = value;
            }
        }

        public Character character
        {
            get
            {
                return playerBehaviourTree.character;
            }
        }

        public Transform cameraTransform
        {
            get
            {
                return playerBehaviourTree.cameraTransform;
            }
        }

        public Canvas canvas
        {
            get
            {
                return playerBehaviourTree.canvas;
            }
        }

        public InputHandler inputHandler
        {
            get
            {
                return playerBehaviourTree.inputHandler;
            }
        }

        public Vector3 additiveVelocity
        {
            get
            {
                return playerBehaviourTree.additiveVelocity;
            }
            set
            {
                playerBehaviourTree.additiveVelocity = value;
            }
        }

        public Vector3 movePosition
        {
            get
            {
                return playerBehaviourTree.movePosition;
            }
            set
            {
                playerBehaviourTree.movePosition = value;
            }
        }

        public Vector3 moveVelocity
        {
            get
            {
                return playerBehaviourTree.moveVelocity;
            }
            set
            {
                playerBehaviourTree.moveVelocity = value;
            }
        }

        public Vector3 accelerationVector
        {
            get
            {
                return playerBehaviourTree.accelerationVector;
            }
            set
            {
                playerBehaviourTree.accelerationVector = value;
            }
        }

        public Vector3 tangent
        {
            get
            {
                return playerBehaviourTree.tangent;
            }
            set
            {
                playerBehaviourTree.tangent = value;
            }
        }

        public Vector3 plane
        {
            get
            {
                return playerBehaviourTree.plane;
            }
            set
            {
                playerBehaviourTree.plane = value;
            }
        }

        public bool overrideModelTransform
        {
            get
            {
                return playerBehaviourTree.overrideModelTransform;
            }
            set
            {
                playerBehaviourTree.overrideModelTransform = value;
            }
        }

        public bool kinematic
        {
            get
            {
                return playerBehaviourTree.kinematic;
            }
            set
            {
                playerBehaviourTree.kinematic = value;
            }
        }

        public bool respawnTrigger
        {
            get
            {
                return playerBehaviourTree.respawnTrigger;
            }
            set
            {
                playerBehaviourTree.respawnTrigger = value;
            }
        }

        public bool moving
        {
            get
            {
                return playerBehaviourTree.moving;
            }
            set
            {
                playerBehaviourTree.moving = value;
            }
        }
    }
}
