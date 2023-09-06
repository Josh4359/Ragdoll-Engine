using UnityEngine;

namespace RagdollEngine
{
    public class BoostPlayerBehaviour : PlayerBehaviour
    {
        public float speed;

        [SerializeField] BoostUI boostUI;

        [SerializeField] float turnSensitivity;

        [SerializeField] float boostDeadzone;

        [SerializeField] float inactiveBoostDepletionSpeed;

        [SerializeField] float activeBoostDepletionSpeed;

        [SerializeField] float centrifugalForce;

        [HideInInspector] public bool boosting;

        [HideInInspector] public bool hold;

        [HideInInspector] public float boostPercent;

        BoostUI currentBoostUI;

        void LateUpdate()
        {
            animator.SetBool("Boosting", active);
        }

        void FixedUpdate()
        {
            boostPercent = Mathf.Max(boostPercent - ((boosting ? activeBoostDepletionSpeed : inactiveBoostDepletionSpeed) * Time.fixedDeltaTime), 0);
        }

        public override bool Evaluate()
        {
            if (boostPercent <= 0)
            {
                boosting = false;

                return false;
            }

            bool wasBoost = hold;

            hold = false;

            boosting = ((inputHandler.boost.pressed && groundInformation.ground)
                    || ((boosting || wasBoost) && inputHandler.boost.hold))
                && (!boosting || RB.velocity.magnitude >= boostDeadzone);

            if (!currentBoostUI && boostPercent > 0)
            {
                currentBoostUI = Instantiate(boostUI, canvas.transform);

                character.uis.Add(currentBoostUI.gameObject);

                currentBoostUI.Initialize(this);
            }

            return boosting;
        }

        public override void Execute()
        {
            if (!active) return;

            Vector3 moveForwardNormal = moveVelocity.magnitude > moveDeadzone ? moveVelocity.normalized : Vector3.ProjectOnPlane(modelTransform.forward, playerTransform.up);

            Vector3 moveRightNormal = Vector3.Cross(playerTransform.up, moveForwardNormal).normalized;

            Vector3 moveNormal = (moveForwardNormal * inputHandler.move.y)
                + (moveRightNormal * inputHandler.move.x);

            Vector3 moveMask = Vector3.Project(moveNormal, plane);

            additiveVelocity = -moveVelocity
                + ((moveForwardNormal * speed * (1 - turnSensitivity))
                    + ((moveNormal - moveMask) * speed * turnSensitivity))
                + (Vector3.Cross(moveForwardNormal, playerTransform.up).normalized
                    * inputHandler.move.x
                    * centrifugalForce);
        }
    }
}
