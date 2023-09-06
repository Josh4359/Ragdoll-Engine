using UnityEngine;

public class Collectible : Entity
{
    [SerializeField] Animator animator;

    [HideInInspector] public bool collected;

    public virtual void OnCollect() { }

    public void Collect()
    {
        if (collected) return;

        animator.Play("Break");

        collected = true;
    }
}
