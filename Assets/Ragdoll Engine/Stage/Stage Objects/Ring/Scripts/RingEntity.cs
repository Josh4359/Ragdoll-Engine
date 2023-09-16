using RagdollEngine;
using System.Collections;
using UnityEngine;

public class RingEntity : Entity
{
    [SerializeField] AudioSourceInstance ringAudioSourceInstance;

    [SerializeField] Animator animator;

    [SerializeField] float animationTime;

    [SerializeField] float animationSpinDegrees;

    [SerializeField] float animationHeight;

    bool collected;

    public void Collect(PlayerBehaviourTree playerBehaviourTree)
    {
        if (collected) return;

        collected = true;

        Rings.AddRings(playerBehaviourTree, 1);

        Instantiate(ringAudioSourceInstance);

        //animator.SetTrigger("Collect");

        StartCoroutine(PlayLerpAnimation());

        IEnumerator PlayLerpAnimation()
        {
            Vector3 positionOffset = transform.position - playerBehaviourTree.playerTransform.position;

            float animationTimer = animationTime;

            while (animationTimer > 0)
            {
                animationTimer -= Time.deltaTime;

                float lerp = 1 - (animationTimer / animationTime);

                Vector3 thisPositionOffset = Quaternion.AngleAxis(animationSpinDegrees * lerp, Vector3.up) * positionOffset;

                transform.position = Vector3.Lerp(playerBehaviourTree.playerTransform.position + thisPositionOffset, playerBehaviourTree.playerTransform.position, lerp)
                    + (Vector3.up * animationHeight * Mathf.Sin(lerp * Mathf.PI));

                yield return null;
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerBehaviourTree playerBehaviourTree = other.transform.parent.GetComponentInChildren<PlayerBehaviourTree>();

        if (!playerBehaviourTree) return;

        Collect(playerBehaviourTree);
    }
}
