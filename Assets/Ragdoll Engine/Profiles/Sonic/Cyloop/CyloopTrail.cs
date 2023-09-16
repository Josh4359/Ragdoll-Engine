using RagdollEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class CyloopTrail : MonoBehaviour
{
    public Color color;

    [Range(0, 1)] public float colorSlider;

    [SerializeField] Animator animator;

    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] int maxLength;

    [SerializeField] float minVertexDistance;

    [SerializeField] float overlapDistance;

    [SerializeField] float minOverlapLength;

    CyloopPlayerBehaviour cyloopPlayerBehaviour;

    PlayerBehaviourTree playerBehaviourTree => cyloopPlayerBehaviour.playerBehaviourTree;

    Transform playerTransform => playerBehaviourTree.playerTransform;

    MaterialPropertyBlock materialPropertyBlock;

    class Loop
    {
        public int startIndex;

        public int endIndex;
    }

    List<Loop> loops = new();

    bool initialized;

    bool active;

    bool overlapCooldown;

    bool closed;

    public void Initialize(CyloopPlayerBehaviour cyloopPlayerBehaviour)
    {
        if (initialized) return;

        this.cyloopPlayerBehaviour = cyloopPlayerBehaviour;

        lineRenderer.SetPositions(new Vector3[]
        {
            playerTransform.position,
            playerTransform.position
        });

        materialPropertyBlock = new();

        lineRenderer.GetPropertyBlock(materialPropertyBlock);

        initialized = true;

        active = true;
    }

    void Update()
    {
        if (!initialized || !active) goto UpdateMaterials;

        Vector3[] positions = new Vector3[lineRenderer.positionCount];

        if (!cyloopPlayerBehaviour.active)
        {
            active = false;

            lineRenderer.GetPositions(positions);

            CollisionCheck(positions);

            animator.SetTrigger("Disable");

            goto UpdateMaterials;
        }

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerTransform.position);

        lineRenderer.GetPositions(positions);

        float length = GetLength(positions);

        if (Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 2), playerTransform.position) >= minVertexDistance)
        {
            lineRenderer.positionCount++;

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerTransform.position);

            if (length > maxLength)
            {
                positions = new Vector3[lineRenderer.positionCount];

                lineRenderer.GetPositions(positions);

                length -= Vector3.Distance(positions[0], positions[1]);

                List<Vector3> positionsList = positions.ToList();

                positionsList.RemoveAt(0);

                foreach (Loop thisLoop in loops)
                {
                    thisLoop.startIndex--;

                    thisLoop.endIndex--;
                }

                lineRenderer.positionCount = positionsList.Count;

                lineRenderer.SetPositions(positionsList.ToArray());
            }

            positions = new Vector3[lineRenderer.positionCount];

            lineRenderer.GetPositions(positions);

            if (overlapCooldown)
                overlapCooldown = OverlapCheck(positions);
            else if (OverlapCheck(positions))
            {
                animator.SetTrigger("Flash");

                overlapCooldown = true;
            }
        }

        materialPropertyBlock.SetFloat("_Length", length);

    UpdateMaterials:

        materialPropertyBlock.SetColor("_Color", color);

        lineRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    float GetLength(Vector3[] positions)
    {
        float length = 0;

        for (int i = 1; i < positions.Length; i++)
            length += Vector3.Distance(positions[i - 1], positions[i]);

        return length;
    }

    bool OverlapCheck(Vector3[] positions)
    {
        for (int i = 1; i < positions.Length - minOverlapLength; i++)
        {
            Vector3 point0 = positions[i - 1];

            Vector3 point1 = positions[i];

            Vector3 line = point1 - point0;

            Vector3 difference = playerTransform.position - point0;

            float dot = Vector3.Dot(difference, line.normalized);

            Vector3 position = Vector3.Lerp(point0, point1, dot / line.magnitude);

            if (Vector3.Distance(position, playerTransform.position) <= overlapDistance)
            {
                loops.Add(new()
                {
                    startIndex = i - 1,
                    endIndex = positions.Length
                });

                closed = true;

                return true;
            }
        }

        return false;
    }

    void CollisionCheck(Vector3[] positions)
    {
        if (!closed) return;

        foreach (Loop thisLoop in loops)
        {
            if (thisLoop.startIndex < 0 || thisLoop.endIndex < 0) continue;

            LineRenderer lineRenderer1 = new GameObject().AddComponent<LineRenderer>();

            lineRenderer1.name = "Line Renderer";

            lineRenderer1.alignment = LineAlignment.TransformZ;

            Vector3[] positions1 = new Vector3[thisLoop.endIndex - thisLoop.startIndex];

            for (int i = thisLoop.startIndex; i < thisLoop.endIndex; i++)
                positions1[i - thisLoop.startIndex] = positions[i];

            lineRenderer1.positionCount = thisLoop.endIndex - thisLoop.startIndex;

            lineRenderer1.SetPositions(positions1);

            Mesh mesh = new();

            lineRenderer1.BakeMesh(mesh, playerBehaviourTree.cameraTransform.GetComponentInChildren<Camera>());

            MeshCollider meshCollider = lineRenderer1.gameObject.AddComponent<MeshCollider>();

            meshCollider.convex = true;

            meshCollider.sharedMesh = mesh;

            meshCollider.isTrigger = true;

            foreach (CyloopTarget thisCyloopTarget in FindObjectsOfType<CyloopTarget>())
                if (Physics.ComputePenetration(meshCollider, Vector3.zero, Quaternion.identity, thisCyloopTarget.targetCollider, thisCyloopTarget.transform.position, thisCyloopTarget.transform.rotation, out _, out _))
                    thisCyloopTarget.OnTarget(playerBehaviourTree);

            Destroy(lineRenderer1.gameObject);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
