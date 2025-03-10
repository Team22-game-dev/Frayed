using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Heal_Up,
        Heart_Up
    }

    public PowerUpType powerUpType; // To distinguish between power-up types

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the power-up is the MainCharacter
        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.LogError("Player hit power-up!");

            // Get PlayerStats from the MainCharacter
            PlayerStats playerStats = PlayerStats.Instance;

            if (playerStats != null)
            {
                // Perform action based on the power-up type
                switch (powerUpType)
                {
                    case PowerUpType.Heal_Up:
                        UnityEngine.Debug.LogError("Healing Player...");
                        playerStats.Heal(playerStats.MaxHealth - playerStats.Health); // Heal to max health
                        break;

                    case PowerUpType.Heart_Up:
                        UnityEngine.Debug.LogError("Increasing Health...");
                        playerStats.AddHealth();
                        playerStats.Heal(1); // Increase max health by 1
                        break;
                }

                // Destroy the power-up object after being collected
                Destroy(gameObject);
            }
            else
            {
                UnityEngine.Debug.LogError("PlayerStats not found on Player!");
            }
        }
    }

}
