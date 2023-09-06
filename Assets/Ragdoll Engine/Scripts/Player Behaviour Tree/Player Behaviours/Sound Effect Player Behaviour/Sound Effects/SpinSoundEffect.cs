using UnityEngine;

namespace RagdollEngine
{
    public class SpinSoundEffect : SoundEffect
    {
        [SerializeField] JumpPlayerBehaviour jumpPlayerBehaviour;

        public override bool Evaluate()
        {
            active = jumpPlayerBehaviour.fixedGroundJump;

            return active;
        }
    }
}
