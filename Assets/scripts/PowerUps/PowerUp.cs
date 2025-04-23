using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Heal_Up,
        Heart_Up,
        Fireball
    }

    public PowerUpType powerUpType; // To distinguish between power-up types

    private GameObject player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Assert(player != null);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the power-up is the MainCharacter
        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Player hit power-up!");

            // Get PlayerStats from the MainCharacter
            PlayerStats playerStats = PlayerStats.Instance;

            if (playerStats != null)
            {
                // Perform action based on the power-up type
                switch (powerUpType)
                {
                    case PowerUpType.Heal_Up:
                        Debug.Log("Healing Player...");
                        playerStats.Heal(playerStats.MaxHealth - playerStats.Health); // Heal to max health
                        break;

                    case PowerUpType.Heart_Up:
                        Debug.Log("Increasing Health...");
                        playerStats.Heal(1); // Increase max health by 1
                        break;
                    case PowerUpType.Fireball:
                        Debug.Log("Activating fireball powerup...");
                        if (player.GetComponent<FireBall>() != null)
                        {
                            player.GetComponent<FireBall>().hasPowerup = true;
                            break;
                        }
                        break;
                }

                // Destroy the power-up object after being collected
                //Destroy(gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("PlayerStats not found on Player!");
            }
        }
    }

}
