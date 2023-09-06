using UnityEngine;

namespace RagdollEngine
{
    public class DriftSoundEffect : SoundEffect
    {
        [SerializeField] DriftPlayerBehaviour driftPlayerBehaviour;

        public override bool Evaluate()
        {
            active = driftPlayerBehaviour.active;

            return active;
        }
    }
}
