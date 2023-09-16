using UnityEngine;

namespace RagdollEngine
{
    public class CyloopTarget : MonoBehaviour
    {
        public Collider targetCollider;

        public virtual void OnTarget(PlayerBehaviourTree playerBehaviourTree) { }
    }
}
