using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceInstance : MonoBehaviour
{
    void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        StartCoroutine(WaitForAudioSource());

        IEnumerator WaitForAudioSource()
        {
            audioSource.Play();

            while (audioSource.isPlaying)
                yield return null;

            Destroy(gameObject);
        }
    }
}
