using UnityEngine;

namespace RagdollEngine
{
    public class JumpRailExtension : RailExtension
    {
        [SerializeField] JumpPlayerBehaviour jumpPlayerBehaviour;

        [SerializeField] AudioSource jumpAudioSource;

        public override void Execute()
        {
            if (inputHandler.jump.pressed)
            {
                playerBehaviourTree.groundInformation.ground = false;

                jumpPlayerBehaviour.Jump(matrix.c1, true);

                jumpAudioSource.Play();

                rail = false;

                pass = true;
            }
        }
    }
}
