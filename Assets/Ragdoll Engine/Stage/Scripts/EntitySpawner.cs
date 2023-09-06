using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] Entity entity;

    [SerializeField] float respawnTime;

    Entity currentEntity;

    void OnEnable()
    {
        Spawn();
    }

    void OnDisable()
    {
        if (currentEntity)
            currentEntity.Destroyed -= OnDestroyed;

        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void Spawn()
    {
        currentEntity = Instantiate(entity, gameObject.transform);

        currentEntity.Destroyed += OnDestroyed;
    }

    void OnDestroyed(object sender, EventArgs e)
    {
        ((Entity)sender).Destroyed -= OnDestroyed;

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        Spawn();
    }
}
