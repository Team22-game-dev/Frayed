using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnItemController : MonoBehaviour
{
    private HashSet<RespawnItem> set;
    private Dictionary<RespawnItem, RespawnItem> cloneDict;

    private void Start()
    {
        set = new HashSet<RespawnItem>();
        cloneDict = new Dictionary<RespawnItem, RespawnItem>();
    }
    void Update()
    {
        foreach (RespawnItem respawnItem in GameObject.FindObjectsOfType<RespawnItem>())
        {
            if (set.Add(respawnItem))
            {
                RespawnItem clone = Instantiate<RespawnItem>(respawnItem, respawnItem.spawnLocation, respawnItem.spawnRotation);
                clone.gameObject.SetActive(false);
                cloneDict[respawnItem] = clone;
            }
        }
        List<RespawnItem> toRemove = new List<RespawnItem>();
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
                    cloneDict[respawnItem].gameObject.SetActive(true);
                    toRemove.Add(respawnItem);
                }
            }
            else if (!respawnItem.gameObject.activeSelf)
            {
                respawnItem.deactivated = true;
                respawnItem.timeDeactivated = 0.0f;
                respawnItem.currentRespawnTime = Random.Range(respawnItem.minRespawnTime, respawnItem.maxRespawnTime);
            }
        }
        foreach (RespawnItem respawnItem in toRemove)
        {
            set.Remove(respawnItem);
            cloneDict.Remove(respawnItem);
            Destroy(respawnItem.gameObject);
        }
    }
}
