using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogEmitter : MonoBehaviour
{
    private EnemyManager enemyManager;

    [SerializeField]
    private GameObject fogEmitterPrefab; // Follow C# naming convention: camelCase for variables

    private EnemyManager.State lastState;

    private float dropBuffer = 1f; // Buffer controls how often fog is dropped
    private float lastEmit;

    void Start()
    {
        // Fix the casing of EnemyManager (was "enemyManager")
        enemyManager = GetComponent<EnemyManager>();

        if (enemyManager == null)
        {
            Debug.LogError("FogEmitter: EnemyManager not found");
        }

        lastState = enemyManager.currentState;
        lastEmit = Time.time;
    }

    void Update()
    {
        if (enemyManager == null) return;

        var currentState = enemyManager.currentState;

        if (lastState != currentState && currentState == EnemyManager.State.CHASING)
        {
            DropFog();
        }
        else if (currentState == EnemyManager.State.CHASING && Time.time - lastEmit > dropBuffer)
        {
            DropFog();
        }

        lastState = currentState;
    }

    private void DropFog()
    {
        Debug.Log("Dropping emitter at: " + gameObject.transform.position);
        if (fogEmitterPrefab != null)
        {
            var spawnPos = transform.position - transform.forward * 0.5f;
            Quaternion rotation = Quaternion.Euler(90.9f, 0f, 0f);

            var fog = Instantiate(fogEmitterPrefab, gameObject.transform.position, rotation);

            var emitter = fog.GetComponent<ParticleSystem>();

            if (emitter != null)
            {
                var emission = emitter.emission;
                emission.enabled = true;
                emitter.Play();
            }
            else
            {
                Debug.LogWarning("FogEmitter: No ParticleSystem found on fogEmitterPrefab.");
            }


            Destroy(fog, 5f); // destroy after 3 seconds


        }
        else
        {
            Debug.LogWarning("FogEmitter: fogEmitterPrefab not assigned.");
        }
    }
}
