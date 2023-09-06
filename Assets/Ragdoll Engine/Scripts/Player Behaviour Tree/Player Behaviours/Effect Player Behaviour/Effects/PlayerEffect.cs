using System;
using UnityEngine;

namespace RagdollEngine
{
    public class PlayerEffect : PlayerBehaviour
    {
        [Serializable]
        public struct Interaction
        {
            public enum InteractionType
            {
                Or,
                Require,
                Exception
            }

            public PlayerBehaviour playerBehaviour;

            public InteractionType interactionType;
        }

        public Interaction[] interactions;

        public GameObject[] gameObjects;

        public ParticleSystem[] particleSystems;

        public TrailRenderer[] trailRenderers;

        public override bool Evaluate()
        {
            bool returning = false;

            bool or = false;

            foreach (Interaction thisInteraction in interactions)
            {
                switch (thisInteraction.interactionType)
                {
                    case Interaction.InteractionType.Or:
                        or = or || thisInteraction.playerBehaviour.active;

                        break;

                    case Interaction.InteractionType.Require:
                        if (thisInteraction.playerBehaviour.active)
                            returning = true;
                        else
                            return false;

                        break;
                    case Interaction.InteractionType.Exception:
                        if (thisInteraction.playerBehaviour.active)
                            return false;
                        else
                            returning = true;

                        break;
                }
            }

            return returning || or;
        }

        public void ToggleEffects(bool value)
        {
            foreach (GameObject thisGameObject in gameObjects)
                thisGameObject.SetActive(value);

            foreach (ParticleSystem thisParticleSystem in particleSystems)
            {
                ParticleSystem.EmissionModule emissionModule = thisParticleSystem.emission;

                emissionModule.enabled = value;
            }

            foreach (TrailRenderer thisTrailRenderer in trailRenderers)
                thisTrailRenderer.emitting = value;
        }
    }
}
