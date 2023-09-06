using RagdollEngine;
using UnityEngine;

public class Aura : MonoBehaviour
{
    [SerializeField] PlayerBehaviourTree playerBehaviourTree;

    Camera distortionCamera => playerBehaviourTree.cameraTransform.GetChild(0).GetComponent<Camera>(); // placeholder

    void OnDisable()
    {
        DistortionEnd();
    }

    public void DistortionStart()
    {
        if (!distortionCamera) return;

        distortionCamera.gameObject.SetActive(true);
    }

    public void DistortionEnd()
    {
        if (!distortionCamera) return;

        distortionCamera.gameObject.SetActive(false);
    }
}
