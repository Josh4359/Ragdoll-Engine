using UnityEngine;

namespace RagdollEngine
{
    public class BoostUI : MonoBehaviour
    {
        [SerializeField] RectTransform guage;

        [SerializeField] RectTransform minTransform;

        [SerializeField] RectTransform maxTransform;

        [SerializeField] Animator animator;

        BoostPlayerBehaviour boostPlayerBehaviour;

        bool active = true;

        void Update()
        {
            if (!boostPlayerBehaviour) return;

            if (boostPlayerBehaviour.boostPercent <= 0)
                animator.SetTrigger("Disable");

            if (!active) return;

            guage.position = Vector3.Lerp(minTransform.position, maxTransform.position, boostPlayerBehaviour.boostPercent);
        }

        public void Initialize(BoostPlayerBehaviour boostPlayerBehaviour)
        {
            this.boostPlayerBehaviour = boostPlayerBehaviour;
        }

        public void Enter()
        {
            active = true;
        }

        public void Destoy()
        {
            Destroy(gameObject);
        }
    }
}
