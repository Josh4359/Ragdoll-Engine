using UnityEngine;

public class SpringStageObject : StageObject
{
    public AudioSource audioSource;

    public float speed;

    public float length;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.up * length);
    }

    /*
    public AudioSource audioSource;

    public Vector3 goal;

    [SerializeField] Animator animator;

    [SerializeField] int resolution;

    [SerializeField] float _height;

    public float height
    {
        get
        {
            return Mathf.Max(goal.y, _height);
        }
    }

    void Awake()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up,
            Vector3.Lerp(Vector3.zero, goal, 0.5f / resolution)
                + (Vector3.up * Mathf.Sin(0.5f / resolution * Mathf.PI) * (height - (goal.y / height))));
    }

    public void Spring()
    {
        animator.Play("Spring");
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < resolution; i++)
        {
            Vector3 from = Vector3.Lerp(transform.position, transform.position + goal, (float)i / resolution)
                + (Vector3.up * Mathf.Sin((float)i / resolution * Mathf.PI) * height);

            Vector3 to = Vector3.Lerp(transform.position, transform.position + goal, (float)(i + 1) / resolution)
                + (Vector3.up * Mathf.Sin((float)(i + 1) / resolution * Mathf.PI) * height);

            Gizmos.DrawLine(from, to);
        }
    }
    */
}
