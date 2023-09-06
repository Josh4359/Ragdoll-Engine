using UnityEngine;

namespace RagdollEngine
{
    public class GravityPlayerBehaviour : PlayerBehaviour
    {
        public float gravity;

        public override void Execute()
        {
            additiveVelocity += Vector3.ProjectOnPlane(-Vector3.up, plane).normalized * gravity;
        }
    }
}
