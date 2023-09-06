using UnityEngine;

namespace RagdollEngine
{
    public class BoostCapsuleHomingTarget : HomingTarget
    {
        [SerializeField] float bounciness;

        public override void OnTarget(PlayerBehaviourTree playerBehaviourTree)
        {
            playerBehaviourTree.RB.velocity = new Vector3(playerBehaviourTree.RB.velocity.x,
                bounciness,
                playerBehaviourTree.RB.velocity.z);
        }
    }
}
