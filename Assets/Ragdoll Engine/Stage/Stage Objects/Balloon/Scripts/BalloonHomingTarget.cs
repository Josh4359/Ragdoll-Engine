using UnityEngine;

namespace RagdollEngine
{
    [RequireComponent(typeof(BalloonEntity))]
    public class BalloonHomingTarget : HomingTarget
    {
        [SerializeField] float bounciness;

        BalloonEntity balloonEntity;

        void Awake()
        {
            balloonEntity = GetComponent<BalloonEntity>();
        }

        public override void OnTarget(PlayerBehaviourTree playerBehaviourTree)
        {
            playerBehaviourTree.RB.velocity = new Vector3(playerBehaviourTree.RB.velocity.x,
                bounciness,
                playerBehaviourTree.RB.velocity.z);

            balloonEntity.Break(playerBehaviourTree);
        }
    }
}
