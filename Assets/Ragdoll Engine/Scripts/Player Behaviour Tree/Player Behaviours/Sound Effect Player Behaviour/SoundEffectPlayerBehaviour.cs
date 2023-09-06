using UnityEngine;

namespace RagdollEngine
{
    public class SoundEffectPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] SoundEffect[] soundEffects;

        public override void Execute()
        {
            foreach (SoundEffect thisSoundEffect in soundEffects)
                if (thisSoundEffect.Check() && !thisSoundEffect.wasActive)
                    thisSoundEffect.Execute();
        }
    }
}
