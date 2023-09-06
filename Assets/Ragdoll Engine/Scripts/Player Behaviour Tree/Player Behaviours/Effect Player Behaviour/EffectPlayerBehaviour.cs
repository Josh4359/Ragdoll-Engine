using System;
using System.Collections.Generic;
using UnityEngine;

namespace RagdollEngine
{
    public class EffectPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] List<Effect> effects = new List<Effect>();

        [SerializeField] GameObject root;

        [Serializable]
        struct Effect
        {
            public PlayerEffect playerEffect;

            public bool replace;
        }

        public override void Execute()
        {
            bool replace = false;

            foreach (Effect thisEffect in effects)
            {
                bool evaluation = thisEffect.playerEffect.Evaluate();

                if (evaluation)
                    replace = replace || thisEffect.replace;

                thisEffect.playerEffect.ToggleEffects(evaluation);
            }

            root.SetActive(!replace);
        }
    }
}
