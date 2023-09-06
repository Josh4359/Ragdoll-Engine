using System;
using UnityEngine;

namespace RagdollEngine
{
    public class SoundEffect : PlayerBehaviour
    {
        [SerializeField] Effect[] effects;

        [Serializable]
        struct Effect
        {
            public AudioSource audioSource;

            public bool stopOnInactive;
        }

        public override bool Evaluate()
        {
            return true;
        }

        public override void Execute()
        {
            foreach (Effect thisEffect in effects)
                thisEffect.audioSource.Play();
        }

        public bool Check()
        {
            bool returning = Evaluate();

            if (!returning)
                foreach (Effect thisEffect in effects)
                    if (thisEffect.stopOnInactive)
                        thisEffect.audioSource.Stop();

            return returning;
        }
    }
}
