using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogEmitter : MonoBehaviour
{
    private EnemyManager enemyManager;

    [SerializeField]
    private GameObject fogEmitterPrefab;

    private EnemyManager.State lastState;

    private int maxEmitters = 5;
    private int emitters = 0;

    private float dropBuffer = 2f; // Buffer controls how often fog is dropped
    private float lastEmit;

    void Start()
    {
        enemyManager = GetComponent<EnemyManager>();

        if (enemyManager == null)
        {
            Debug.LogError("FogEmitter: EnemyManager not found");
        }

        lastState = enemyManager.currentState;
        lastEmit = Time.time; // Initialize the lastEmit time at the start
    }

    void Update()
    {
        if (enemyManager == null) return;

        var currentState = enemyManager.currentState;

        // Check if the state has changed to CHASING or if enough time has passed since the last fog emission
        if ((lastState != currentState && currentState == EnemyManager.State.CHASING) || 
            (currentState == EnemyManager.State.CHASING && Time.time - lastEmit > dropBuffer))
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
            // Using the spawnPos to offset the fog from the emitter
            var spawnPos = transform.position + transform.forward * 2.5f;
            Quaternion rotation = Quaternion.Euler(90.9f, 0f, 0f);

            // Instantiate fog at the desired position (spawnPos)
            if(emitters < maxEmitters)
            {
                var fog = Instantiate(fogEmitterPrefab, spawnPos, rotation);
                
                emitters++;
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

                // Destroy the fog after 5 seconds and decrement the emitter count
                Destroy(fog, 5f); 
                lastEmit = Time.time; // Update the lastEmit time after emitting fog

                // Using the Destroy callback
                StartCoroutine(DecrementEmitterAfterDelay(fog));
            }
        }
        else
        {
            Debug.LogWarning("FogEmitter: fogEmitterPrefab not assigned.");
        }
    }

    private IEnumerator DecrementEmitterAfterDelay(GameObject fog)
    {
        // Wait for the fog to be destroyed
        yield return new WaitForSeconds(5f);

        // Only decrement if the fog is destroyed (optional check for null)
        if (fog == null)
        {
            emitters--;
        }
    }
}
