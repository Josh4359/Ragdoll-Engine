using UnityEngine;

namespace RagdollEngine
{
    public class StompSoundEffect : SoundEffect
    {
        [SerializeField] StompPlayerBehaviour stompPlayerBehaviour;

        public override bool Evaluate()
        {
            active = stompPlayerBehaviour.active;

            return active;
        }
    }
}
