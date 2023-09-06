using UnityEngine;

namespace RagdollEngine
{
    public class RunPlayerEffect : PlayerEffect
    {
        [SerializeField] float speed;

        [SerializeField] float dustKickSpeedRatio;

        public override bool Evaluate()
        {
            if (!base.Evaluate()) return false;

            foreach (ParticleSystem thisParticleSystem in particleSystems)
            {
                ParticleSystem.MainModule mainModule = thisParticleSystem.main;

                mainModule.startSpeed = Vector3.Dot(RB.velocity, thisParticleSystem.transform.forward) - (RB.velocity.magnitude * dustKickSpeedRatio);
            }

            return RB.velocity.magnitude > speed && groundInformation.ground;
        }
    }
}
