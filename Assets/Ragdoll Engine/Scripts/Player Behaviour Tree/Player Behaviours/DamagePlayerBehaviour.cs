using UnityEngine;

namespace RagdollEngine
{
    public class DamagePlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] AudioSource audioSource;

        [SerializeField] AudioClip knockbackAudioClip;

        [SerializeField] AudioClip stumbleAudioClip;

        [SerializeField] AudioClip ringSpreadAudioClip;

        [SerializeField] ParticleSystem ringScatterEffectPrefab;

        [SerializeField] float knockbackDistance;

        [SerializeField] float knockbackTime;

        [SerializeField] float knockbackHeight;

        [SerializeField] float stumbleSpeedMultiplier;

        [SerializeField] float minStumbleSpeed;

        [SerializeField] float knockbackAnimationTime;

        [SerializeField] float stumbleAnimationTime;

        [SerializeField] float cooldownTime;

        public enum DamageMode
        {
            Knockback,
            Stumble
        }

        DamageMode damageMode;

        Vector3 knockbackDirection;

        bool knockback;

        bool stumbling;

        bool damageCheck;

        float knockbackTimer;

        float animationTimer;

        float cooldownTimer;

        void LateUpdate()
        {
            stumbling = stumbling && animationTimer > 0;

            animator.SetBool("Stumbling", stumbling);
        }

        public override bool Evaluate()
        {
            cooldownTimer = Mathf.Max(cooldownTimer - Time.fixedDeltaTime, 0);

            return EvaluateDamage() || DamageCheck();
        }

        void Knockback()
        {
            overrideModelTransform = true;

            knockbackTimer -= Time.fixedDeltaTime;

            float t = Mathf.Clamp01(1 - (knockbackTimer / knockbackTime));

            Vector3 up = groundInformation.ground ? groundInformation.hit.normal : Vector3.up;

            modelTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-knockbackDirection, up), up);

            modelTransform.position = (groundInformation.cast ? groundInformation.hit.point : playerTransform.position - (modelTransform.up * height))
                + (up * knockbackHeight * Mathf.Sin(t * Mathf.PI));

            if (!knockback || knockbackTimer < 0)
            {
                if (groundInformation.ground)
                    additiveVelocity = -RB.velocity;

                return;
            }

            if (!groundInformation.cast)
            {
                knockback = false;

                return;
            }

            additiveVelocity = -RB.velocity
                + (knockbackDirection * knockbackDistance / knockbackTime);
        }

        bool EvaluateDamage()
        {
            if (!wasActive) return false;

            animationTimer -= Time.fixedDeltaTime;

            if (animationTimer <= 0) return false;

            switch (damageMode)
            {
                case DamageMode.Knockback:
                    Knockback();

                    break;
                case DamageMode.Stumble:
                    if (RB.velocity.magnitude < minStumbleSpeed || !groundInformation.ground)
                    {
                        animationTimer = 0;

                        return false;
                    }

                    break;
            }

            return true;
        }

        bool DamageCheck()
        {
            bool wasDamageCheck = damageCheck;

            damageCheck = false;

            if (cooldownTimer > 0) return false;

            foreach (Volume thisVolume in volumes)
                if (thisVolume is DamageVolume)
                {
                    DamageVolume thisDamageVolume = thisVolume as DamageVolume;

                    damageMode = thisDamageVolume.damageMode;

                    switch (damageMode)
                    {
                        case DamageMode.Knockback:
                            knockbackTimer = knockbackTime;

                            animationTimer = knockbackAnimationTime;

                            cooldownTimer = cooldownTime;

                            knockbackDirection = Vector3.ProjectOnPlane(-RB.velocity, Vector3.up).normalized;

                            if (knockbackDirection.magnitude == 0)
                                knockbackDirection = -modelTransform.forward;

                            knockback = true;

                            stumbling = false;

                            Knockback();

                            audioSource.PlayOneShot(knockbackAudioClip);

                            animator.SetTrigger("Knockback");

                            break;
                        default:
                            if (wasDamageCheck || RB.velocity.magnitude < minStumbleSpeed)
                            {
                                damageCheck = true;

                                continue;
                            }

                            knockbackTimer = 0;

                            animationTimer = stumbleAnimationTime;

                            cooldownTimer = 0;

                            knockback = false;

                            stumbling = true;

                            additiveVelocity = -RB.velocity * (1 - stumbleSpeedMultiplier);

                            audioSource.PlayOneShot(stumbleAudioClip);

                            animator.SetTrigger("Stumble");

                            break;
                    }

                    if (thisDamageVolume.power > 0)
                    {
                        if (!Rings.HasRings(playerBehaviourTree) || Rings.GetRings(playerBehaviourTree) == 0)
                            character.Respawn();
                        else
                        {
                            int rings = Rings.GetRings(playerBehaviourTree);

                            int lost = Mathf.FloorToInt(thisDamageVolume.power);

                            Rings.AddRings(playerBehaviourTree, -lost);

                            audioSource.PlayOneShot(ringSpreadAudioClip);

                            ParticleSystem ringScatterEffect = Instantiate(ringScatterEffectPrefab);

                            ringScatterEffect.transform.position = playerTransform.position;

                            ringScatterEffect.emission.SetBursts(new ParticleSystem.Burst[]
                                {
                                    new()
                                    {
                                        count = Mathf.Min(rings, lost)
                                    }
                                });

                            ringScatterEffect.Play();
                        }
                    }

                    damageCheck = true;

                    return true;
                }

            return false;
        }
    }
}
