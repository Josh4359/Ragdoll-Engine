using UnityEngine;

namespace RagdollEngine
{
    public class HomingSoundEffect : SoundEffect
    {
        [SerializeField] HomingAttackPlayerBehaviour homingAttackPlayerBehaviour;

        public override bool Evaluate()
        {
            active = homingAttackPlayerBehaviour.active;

            return active;
        }
    }
}
