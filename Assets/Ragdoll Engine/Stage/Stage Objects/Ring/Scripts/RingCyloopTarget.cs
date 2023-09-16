using RagdollEngine;
using UnityEngine;

[RequireComponent(typeof(RingEntity))]
public class RingCyloopTarget : CyloopTarget
{
    RingEntity ringEntity;

    void Awake()
    {
        ringEntity = GetComponent<RingEntity>();
    }

    public override void OnTarget(PlayerBehaviourTree playerBehaviourTree)
    {
        ringEntity.Collect(playerBehaviourTree);
    }
}
