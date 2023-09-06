using UnityEngine;

public class BoostCapsuleCollectible : Collectible
{
    [SerializeField] AudioSource audioSource;

    public void PlayBreak()
    {
        audioSource.Play();
    }
}