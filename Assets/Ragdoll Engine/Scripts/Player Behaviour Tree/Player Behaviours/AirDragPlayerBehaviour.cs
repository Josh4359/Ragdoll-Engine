using UnityEngine;

namespace RagdollEngine
{
    public class AirDragPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float airDrag;

        public override void Execute()
        {
            additiveVelocity -= RB.velocity * airDrag;
        }
    }
}
