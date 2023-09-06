using System.Collections.Generic;
using UnityEngine;

namespace RagdollEngine
{
    public class CameraPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] List<PlayerCameraState> playerCameraStates;

        PlayerCameraState currentPlayerCameraState;

        public override void Execute()
        {
            foreach (PlayerCameraState thisPlayerCameraState in playerCameraStates)
            {
                if (thisPlayerCameraState.Check())
                {
                    if (currentPlayerCameraState != thisPlayerCameraState)
                    {
                        if (currentPlayerCameraState == null)
                            thisPlayerCameraState.Enable();
                        else
                            thisPlayerCameraState.Transition();

                        currentPlayerCameraState = thisPlayerCameraState;
                    }

                    thisPlayerCameraState.Execute();

                    break;
                }
            }
        }
    }

    public class PlayerCameraState : PlayerBehaviour
    {
        public float transitionTime;

        [HideInInspector] public Quaternion oldRotation;

        [HideInInspector] public Vector3 oldPosition;

        [HideInInspector] public float transition;

        public virtual void FixedUpdate()
        {
            transition = Mathf.Max(transition - Time.fixedDeltaTime, 0);
        }

        public virtual bool Check()
        {
            return true;
        }

        public virtual void Enable() { }

        public virtual void Transition()
        {
            oldRotation = cameraTransform.rotation;

            oldPosition = cameraTransform.position - modelTransform.position;

            transition = transitionTime;
        }
    }
}
