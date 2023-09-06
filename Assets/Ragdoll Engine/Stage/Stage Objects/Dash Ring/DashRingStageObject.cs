using UnityEngine;

public class DashRingStageObject : StageObject
{
    public AudioSource audioSource;

    public float speed;

    public float length;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * length);
    }
}
