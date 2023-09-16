using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RagdollEngine
{
    public class Character : MonoBehaviour
    {
        public Transform cameraTransform;

        public Canvas canvas;

        public InputHandler inputHandler;

        public PlayerBehaviourTree playerBehaviourTreePrefab;

        [SerializeField] RespawnUI respawnUIPrefab;

        [HideInInspector] public List<GameObject> uis;

        [HideInInspector] public PlayerBehaviourTree playerBehaviourTree;

        [HideInInspector] public Utility.TransformData respawnTransformData;

        public void Initialize()
        {
            respawnTransformData = new Utility.TransformData
            {
                position = transform.position,

                rotation = transform.rotation.eulerAngles
            };

            Spawn();
        }

        public virtual void Spawn()
        {
            if (playerBehaviourTree)
                Destroy(playerBehaviourTree.gameObject);

            playerBehaviourTree = Instantiate(playerBehaviourTreePrefab, transform);

            playerBehaviourTree.transform.position = respawnTransformData.position;

            playerBehaviourTree.transform.rotation = Quaternion.Euler(respawnTransformData.rotation);

            cameraTransform.rotation = Quaternion.Euler(respawnTransformData.rotation);

            playerBehaviourTree.Initialize(this);
        }

        public void Respawn()
        {
            StartCoroutine(RespawnRoutine());

            IEnumerator RespawnRoutine()
            {
                RespawnUI respawnUI = Instantiate(respawnUIPrefab, canvas.transform);

                yield return respawnUI.WaitForEnterTransition();

                foreach (GameObject thisUI in uis)
                    Destroy(thisUI.gameObject);

                uis.Clear();

                Spawn();

                respawnUI.Exit();
            }
        }
    }
}
