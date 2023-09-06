using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    public Volume[] volumes
    {
        get
        {
            return GetComponentsInParent<Volume>();
        }
    }
}
