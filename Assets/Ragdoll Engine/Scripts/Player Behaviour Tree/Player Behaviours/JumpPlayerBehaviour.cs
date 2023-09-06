using UnityEngine;

namespace RagdollEngine
{
    public class JumpPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] int jumps;

        [SerializeField] float jumpHeight;

        [SerializeField] float baseJumpHeight;

        [SerializeField] float airJumpHeight;

        [SerializeField] float gravity;

        [SerializeField] float jumpTimer;

        [SerializeField] float coyoteTime;

        [HideInInspector] public bool fixedJump;

        [HideInInspector] public bool fixedGroundJump;

        Vector3 currentJumpForce;

        int currentJumps;

        bool jumping;

        float currentJumpTimer;

        float currentCoyoteTime;

        float jumpForce
        {
            get
            {
                return Mathf.Sqrt(2 * (gravity / Time.fixedDeltaTime) * jumpHeight);
            }
        }

        float airJumpForce
        {
            get
            {
                return Mathf.Sqrt(2 * (gravity / Time.fixedDeltaTime) * airJumpHeight);
            }
        }

        float baseJumpForce
        {
            get
            {
                return Mathf.Sqrt(2 * (gravity / Time.fixedDeltaTime) * baseJumpHeight);
            }
        }

        void LateUpdate()
        {
            animator.ResetTrigger("Jump");
        }

        public override void Execute()
        {
            fixedJump = false;

            fixedGroundJump = false;

            if (groundInformation.ground)
            {
                currentJumps = jumps;

                currentCoyoteTime = coyoteTime;
            }
            else
            {
                if (currentCoyoteTime > 0)
                {
                    currentJumps = jumps;

                    currentCoyoteTime -= Time.fixedDeltaTime;
                }
                else if (currentJumps == jumps)
                    currentJumps--;
            }

            if (jumping)
            {
                if (inputHandler.jump.hold && currentJumpTimer > 0)
                {
                    additiveVelocity = currentJumpForce * (Time.fixedDeltaTime / jumpTimer);

                    currentJumpTimer -= Time.fixedDeltaTime;
                }
                else
                    jumping = false;
            }

            if (inputHandler.jump.pressed)
            {
                if (currentJumps > 0)
                {
                    bool groundJump = groundInformation.ground || currentCoyoteTime > 0;

                    Jump(groundInformation.ground ? groundInformation.hit.normal : Vector3.up, groundJump);

                    if (groundJump)
                    {
                        fixedGroundJump = true;

                        animator.SetTrigger("Jump");
                    }

                    playerBehaviourTree.groundInformation.ground = false;
                }
            }
        }

        public void Jump(Vector3 normal, bool groundJump)
        {
            additiveVelocity = -Vector3.Project(RB.velocity, normal)
                + (normal * Mathf.Max(groundJump ? baseJumpForce : airJumpForce, Vector3.Dot(RB.velocity, normal)));

            if (groundJump)
            {
                currentJumps = jumps - 1;

                jumping = true;

                currentJumpTimer = jumpTimer;
            }
            else
            {
                currentJumps--;

                animator.SetTrigger("Jump");
            }

            currentJumpForce = normal * (groundJump ? jumpForce - baseJumpForce : airJumpForce - airJumpForce);

            currentCoyoteTime = 0;

            fixedJump = true;
        }
    }
}
