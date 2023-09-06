using UnityEngine;

namespace RagdollEngine
{
    public class JumpSoundEffect : SoundEffect
    {
        [SerializeField] JumpPlayerBehaviour jumpPlayerBehaviour;

        public override bool Evaluate()
        {
            active = jumpPlayerBehaviour.fixedJump;

            return active;
        }
    }
}
