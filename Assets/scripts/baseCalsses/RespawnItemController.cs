using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnItemController : MonoBehaviour
{

    private HashSet<RespawnItem> set;

    private void Start()
    {
        set = new HashSet<RespawnItem>();
    }
    void Update()
    {
        foreach (RespawnItem respawnItem in GameObject.FindObjectsOfType<RespawnItem>())
        {
            set.Add(respawnItem);
        }
        foreach (RespawnItem respawnItem in set)
        {
            if (respawnItem == null)
            {
                continue;
            }
            if (respawnItem.deactivated)
            {
                respawnItem.timeDeactivated += Time.deltaTime;
                if (respawnItem.timeDeactivated >= respawnItem.currentRespawnTime)
                {
                    EnemyData enemyData = respawnItem.gameObject.GetComponent<EnemyData>();
                    if (enemyData != null)
                    {
                        enemyData.currentHealth = enemyData.baseHealth;
                    }
                    respawnItem.transform.position = respawnItem.spawnLocation;
                    respawnItem.gameObject.SetActive(true);
                    respawnItem.deactivated = false;
                }
            }
            else if (!respawnItem.gameObject.activeSelf)
            {
                respawnItem.deactivated = true;
                respawnItem.timeDeactivated = 0.0f;
                respawnItem.currentRespawnTime = Random.Range(respawnItem.minRespawnTime, respawnItem.maxRespawnTime);
            }
        }
    }
}
