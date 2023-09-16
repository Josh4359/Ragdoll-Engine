using RagdollEngine;
using UnityEngine;

public class RingCounter : MonoBehaviour
{
    [SerializeField] PlayerBehaviourTree playerBehaviourTree;

    [SerializeField] RingsUI ringsUIPrefab;

    void Update()
    {
        if (!playerBehaviourTree.initialized) return;

        RingsUI ringsUI = Instantiate(ringsUIPrefab, playerBehaviourTree.canvas.transform);

        playerBehaviourTree.character.uis.Add(ringsUI.gameObject);

        ringsUI.Initialize(playerBehaviourTree);

        Destroy(gameObject);
    }
}
