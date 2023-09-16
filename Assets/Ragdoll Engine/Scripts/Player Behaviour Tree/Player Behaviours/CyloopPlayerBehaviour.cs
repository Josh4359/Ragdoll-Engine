using RagdollEngine;
using UnityEngine;

public class CyloopPlayerBehaviour : PlayerBehaviour
{
    [SerializeField] AudioSource[] audioSources;

    [SerializeField] CyloopTrail cyloopTrailPrefab;

    CyloopTrail cyloopTrail;

    void LateUpdate()
    {
        if (!active)
            foreach (AudioSource audioSource in audioSources)
                audioSource.Stop();
    }

    public override void Execute()
    {
        active = (inputHandler.cyloop.pressed || (wasActive && inputHandler.cyloop.hold))
            && plane.magnitude == 0;

        if (active && !wasActive)
        {
            cyloopTrail = Instantiate(cyloopTrailPrefab);

            cyloopTrail.Initialize(this);

            foreach (AudioSource audioSource in audioSources)
                audioSource.Play();
        }
    }
}
