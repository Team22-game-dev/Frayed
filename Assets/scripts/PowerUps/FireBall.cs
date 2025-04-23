using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField]
    private GameObject fireballPrefab;
    [SerializeField]
    private float fireballSpeed = 15f;
    [SerializeField]
    private float cooldown = 0.5f;
    [SerializeField]
    private float life = 5.0f;

    public bool hasPowerup;

    private float lastFireball = -1f;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        hasPowerup = false;
    }

    void Update()
    {
        if (inputManager.shootFireball && hasPowerup)
        {
            // Wait at least `cooldown` seconds between throws
            if (Time.time - lastFireball >= cooldown)
            {
                ThrowFireball();
                lastFireball = Time.time;
            }
        }
    }

    private void ThrowFireball()
    {
        Vector3 spawnPosition = transform.position + transform.up * 1.2f + transform.forward * 0.5f;
        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, transform.rotation);

        Debug.DrawRay(fireball.transform.position, transform.forward * 5, Color.red, 2f);


        // Get Rigidbody from child of prefab
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("Velocity: " + transform.forward * fireballSpeed);
            rb.velocity = transform.forward * fireballSpeed;


        }
        else
        {
            Debug.LogError("rb not found");
        }

        Destroy(fireball, life);

        if (CorruptionMeter.Instance != null)
        {
            CorruptionMeter.Instance.Increase(10f); // Increase by 10%, adjust to balance
        }
    }
}
