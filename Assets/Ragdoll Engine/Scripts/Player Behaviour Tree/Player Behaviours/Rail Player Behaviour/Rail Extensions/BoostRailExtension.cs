using UnityEngine;

namespace RagdollEngine
{
    public class BoostRailExtension : RailExtension
    {
        [SerializeField] BoostPlayerBehaviour boostPlayerBehaviour;

        public override void Execute()
        {
            if (inputHandler.boost.hold)
            {
                if (boostPlayerBehaviour.boostPercent <= 0) return;

                boostPlayerBehaviour.hold = true;

                velocity = velocity.normalized * boostPlayerBehaviour.speed * Time.fixedDeltaTime;

                active = true;
            }
        }
    }
}
