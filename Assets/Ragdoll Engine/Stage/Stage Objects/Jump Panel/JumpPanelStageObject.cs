using UnityEngine;

public class JumpPanelStageObject : StageObject
{
    public AudioSource audioSource;

    public float speed;

    public float angle;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(angle, -transform.right) * transform.forward * speed);
    }
}
