using RagdollEngine;
using TMPro;
using UnityEngine;

public class RingsUI : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] TextMeshProUGUI ringsTextbox;

    [SerializeField] float activeTime;

    bool initialized;

    bool active;

    int rings;

    float activeTimer;

    public void Initialize(PlayerBehaviourTree playerBehaviourTree)
    {
        Rings.GetRingCounter(playerBehaviourTree).ringsChangedEvent += (sender, e) => OnRingsChangedEvent(sender as Rings.RingCounter, e as Rings.RingCounter.RingsChangedEventArgs);

        rings = Rings.GetRings(playerBehaviourTree);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        if (rings > 0)
            activeTimer -= Time.deltaTime;

        bool wasActive = active;

        active = activeTimer > 0 || rings == 0;

        if (active != wasActive)
        {
            if (active)
            {
                activeTimer = activeTime;

                animator.ResetTrigger("Disable");

                animator.SetTrigger("Enable");
            }
            else
            {
                animator.ResetTrigger("Enable");

                animator.SetTrigger("Disable");
            }
        }

        ringsTextbox.text = rings.ToString("000");

        animator.SetInteger("Rings", rings);
    }

    void OnRingsChangedEvent(Rings.RingCounter sender, Rings.RingCounter.RingsChangedEventArgs e)
    {
        rings = sender.rings;

        activeTimer = activeTime;
    }
}
