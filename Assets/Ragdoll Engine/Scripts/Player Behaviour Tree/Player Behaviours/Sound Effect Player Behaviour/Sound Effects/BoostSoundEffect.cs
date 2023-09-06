using UnityEngine;

namespace RagdollEngine
{
    public class BoostSoundEffect : SoundEffect
    {
        [SerializeField] BoostPlayerBehaviour boostPlayerBehaviour;

        public override bool Evaluate()
        {
            active = boostPlayerBehaviour.boosting || boostPlayerBehaviour.hold;

            return active;
        }
    }
}
