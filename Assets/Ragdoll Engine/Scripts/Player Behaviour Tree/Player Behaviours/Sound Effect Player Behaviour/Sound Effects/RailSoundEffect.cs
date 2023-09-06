using UnityEngine;

namespace RagdollEngine
{
    public class RailSoundEffect : SoundEffect
    {
        [SerializeField] RailPlayerBehaviour railPlayerBehaviour;

        public override bool Evaluate()
        {
            active = railPlayerBehaviour.active && railPlayerBehaviour.onRail;

            return active;
        }
    }
}
