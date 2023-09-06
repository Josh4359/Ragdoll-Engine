using System;
using System.Collections.Generic;
using UnityEngine;

namespace RagdollEngine
{
    public class CheckpointStageObject : StageObject
    {
        public Transform checkpointTransform;

        [SerializeField] GameObject pointLaser;

        [SerializeField] AudioSource audioSource;

        [SerializeField] Animator animatorL;

        [SerializeField] Animator animatorR;

        [SerializeField] bool _2D;

        [HideInInspector] public List<Character> characters;

        public static Action<Character> onCheckpoint;

        public void Checkpoint(CheckpointPlayerBehaviour checkpointPlayerBehaviour)
        {
            onCheckpoint?.Invoke(checkpointPlayerBehaviour.character);

            characters.Add(checkpointPlayerBehaviour.character);

            onCheckpoint += OnCheckpoint;

            void OnCheckpoint(Character character)
            {
                if (characters.Contains(character))
                    characters.Remove(character);
            }

            if (pointLaser)
                pointLaser.SetActive(false);

            if (audioSource)
                audioSource.Play();

            if (animatorL)
                animatorL.Play(_2D ? "Checkpoint 2D" : "Checkpoint");

            if (animatorR)
                animatorR.Play(_2D ? "Checkpoint 2D" : "Checkpoint");
        }
    }
}