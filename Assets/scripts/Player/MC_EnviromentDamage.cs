using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MC_EnviromentDamage : MonoBehaviour
{
    MC_AnimationManager mcAnimationManager;
    public float damagePerSecond = 5f; // Damage without armor
    private float damageCooldown = 1f; // Damage interval
    private bool isTakingDamage = false;


    private int hazardousLayerIndex = -1; // Index of the purple terrain texture

    void Start()
    {
        // Add event listener for scene change.
        SceneManager.activeSceneChanged += OnSceneChange;
        mcAnimationManager = GetComponent<MC_AnimationManager>();
        SetUp();
    }

    void SetUp()
    {
        // Find the index of the purple (hazardous) texture
        hazardousLayerIndex = FindTerrainLayerIndex("PurpleLayer"); // Updated layer name
    }

    void OnSceneChange(Scene current, Scene next)
    {
        SetUp();
    }

    void Update()
    {
        if (hazardousLayerIndex == -1) return; // If no hazardous texture found, exit

        int currentLayerIndex = GetTerrainTextureIndex();
        bool onHazardousGround = (currentLayerIndex == hazardousLayerIndex);

        // if (onHazardousGround && !PlayerStats.Instance.HasArmor())
        if (onHazardousGround)
        {
            if (!isTakingDamage)
            {
                isTakingDamage = true;
                StartCoroutine(ApplyDamageOverTime());
            }
        }
        else if (isTakingDamage) // Stop taking damage when leaving hazardous ground
        {
            isTakingDamage = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (isTakingDamage)
        {
            PlayerStats.Instance.TakeDamage(damagePerSecond);
            mcAnimationManager.EnvironmentPain();            
            yield return new WaitForSeconds(damageCooldown);
        }
    }

    private int GetTerrainTextureIndex()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        Vector3 playerPos = transform.position;
        Vector3 terrainPos = terrain.transform.position;

        // Convert world position to terrain position
        int mapX = Mathf.FloorToInt((playerPos.x - terrainPos.x) / terrainData.size.x * terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt((playerPos.z - terrainPos.z) / terrainData.size.z * terrainData.alphamapHeight);

        float[,,] splatMap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        int maxIndex = 0;
        float maxWeight = 0f;

        for (int i = 0; i < splatMap.GetLength(2); i++)
        {
            if (splatMap[0, 0, i] > maxWeight)
            {
                maxWeight = splatMap[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex; // Return the terrain texture index at player's position
    }

    private int FindTerrainLayerIndex(string layerName)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            return -1;
        }
        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
        {
            if (terrain.terrainData.terrainLayers[i].name == layerName)
            {
                return i;
            }
        }
        return -1;
    }
}
