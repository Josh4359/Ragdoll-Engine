using UnityEngine;

namespace RagdollEngine
{
    public class JumpPlayerEffect : PlayerEffect
    {
        [SerializeField] JumpPlayerBehaviour jumpPlayerBehaviour;

        bool jump;

        public override bool Evaluate()
        {
            if (!base.Evaluate())
            {
                jump = false;

                return false;
            }

            jump = jump || jumpPlayerBehaviour.fixedGroundJump;

            return jump;
        }
    }
}
