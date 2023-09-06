using UnityEngine;

public class HintRingEntity : Entity
{
    public string[] hint;

    [SerializeField] Animator animator;

    [SerializeField] AudioSource audioSource;

    [HideInInspector] public bool touched;

    public void Touch()
    {
        if (touched) return;

        touched = true;

        animator.SetTrigger("Touched");

        audioSource.Play();
    }
}
