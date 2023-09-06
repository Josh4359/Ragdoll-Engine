using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] AudioClip[] walkAudioClips;

    [SerializeField] AudioClip[] runAudioClips;

    [SerializeField] AudioSource audioSource;

    public void Walk()
    {
        audioSource.PlayOneShot(walkAudioClips[Random.Range(0, walkAudioClips.Length - 1)]);
    }

    public void Run()
    {
        audioSource.PlayOneShot(runAudioClips[Random.Range(0, runAudioClips.Length - 1)]);
    }
}
