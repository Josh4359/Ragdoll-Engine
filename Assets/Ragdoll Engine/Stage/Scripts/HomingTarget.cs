using UnityEngine;

namespace RagdollEngine
{
    public class HomingTarget : MonoBehaviour
    {
        [SerializeField] Bounds bounds;

        public virtual bool Target(PlayerBehaviourTree playerBehaviourTree, float maxDistance, float viewDot, float maxHeight, out Vector3 point)
        {
            Bounds worldBounds = new Bounds()
            {
                center = transform.position + bounds.center,
                size = Vector3.Scale(bounds.size, transform.lossyScale)
            };

            if (Vector3.Distance(worldBounds.ClosestPoint(playerBehaviourTree.playerTransform.position), playerBehaviourTree.playerTransform.position) > maxDistance)
            {
                point = Vector3.zero;

                return false;
            }

            point = transform.position;

            return true;
        }

        public virtual void OnTarget(PlayerBehaviourTree playerBehaviourTree) { }

        public virtual void Reset()
        {
            if (TryGetComponent(out MeshRenderer meshRenderer))
            {
                bounds = meshRenderer.bounds;

                bounds.center -= transform.position;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(bounds.center + transform.position, bounds.size);
        }
    }
}
