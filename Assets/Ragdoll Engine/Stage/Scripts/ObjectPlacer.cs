#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectPlacer))]
public class ObjectPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ObjectPlacer objectPlacer = target as ObjectPlacer;

        if (GUILayout.Button("Place"))
            objectPlacer.Place();

        if (GUILayout.Button("Move to Target"))
            objectPlacer.MoveToTarget();
    }
}

public class ObjectPlacer : MonoBehaviour
{
    public PlacementType placementType;
    
    [SerializeField] Transform target;

    [SerializeField] Transform reference;

    [SerializeField] Vector3 positionOffset;

    [SerializeField] Vector3 rotationOffset;

    public enum PlacementType
    {
        GroundSnap,
        ObjectOffset
    }

    public void Place()
    {
        CalculateTransform(out Vector3 position, out Quaternion rotation);

        target.position = position;

        target.rotation = rotation;

        EditorUtility.SetDirty(target.gameObject);
    }

    public void MoveToTarget()
    {
        transform.position = target.position;

        transform.rotation = target.rotation;
    }

    void CalculateTransform(out Vector3 position, out Quaternion rotation)
    {
        switch (placementType)
        {
            case PlacementType.GroundSnap:
                target.gameObject.SetActive(false);

                Physics.Raycast(transform.position, -transform.up, out RaycastHit hit);

                target.gameObject.SetActive(true);

                position = hit.point
                    + (transform.right * positionOffset.x)
                    + (transform.up * positionOffset.y)
                    + (transform.forward * positionOffset.z);

                rotation = Quaternion.LookRotation(transform.forward, hit.normal) * Quaternion.Euler(rotationOffset);

                break;
            case PlacementType.ObjectOffset:
                position = reference.position
                    + (reference.right * positionOffset.x)
                    + (reference.up * positionOffset.y)
                    + (reference.forward * positionOffset.z);

                rotation = reference.rotation * Quaternion.Euler(rotationOffset);

                break;
            default:
                position = target.position;

                rotation = target.rotation;

                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        CalculateTransform(out Vector3 position, out _);

        Gizmos.DrawWireSphere(position, 1);
    }
}

#endif
