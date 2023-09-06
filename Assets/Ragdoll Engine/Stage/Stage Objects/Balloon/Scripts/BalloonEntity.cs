using UnityEngine;

namespace RagdollEngine
{
    public class BalloonEntity : Entity
    {
        [SerializeField] Animator animator;

        [SerializeField] ParticleSystem[] particleSystems;

        [SerializeField] AudioSourceInstance audioSourceInstance;

        public void Break(PlayerBehaviourTree playerBehaviourTree)
        {
            Instantiate(audioSourceInstance).transform.position = transform.position;

            playerBehaviourTree.animator.SetTrigger("Balloon");

            foreach (ParticleSystem thisParticleSystem in particleSystems)
            {
                ParticleSystem newParticleSystem = Instantiate(thisParticleSystem);

                newParticleSystem.transform.position = transform.position;
            }

            animator.Play("Break");
        }
    }
}
