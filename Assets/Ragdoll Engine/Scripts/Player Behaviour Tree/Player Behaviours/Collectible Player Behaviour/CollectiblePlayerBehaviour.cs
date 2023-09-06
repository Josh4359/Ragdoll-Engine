using UnityEngine;

namespace RagdollEngine
{
    public class CollectiblePlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] Collector[] collectors;

        public override void Execute()
        {
            foreach (Collector thisCollector in collectors)
                thisCollector.Execute();
        }
    }

    public class Collector : PlayerBehaviour { }
}
