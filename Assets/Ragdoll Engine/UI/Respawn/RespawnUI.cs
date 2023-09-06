using System.Collections;
using UnityEngine;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] float delay;

    bool active;

    public IEnumerator WaitForEnterTransition()
    {
        yield return active;

        yield return new WaitForSeconds(delay);
    }

    public void Enter()
    {
        active = true;
    }

    public void Exit()
    {
        animator.SetTrigger("Disable");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
