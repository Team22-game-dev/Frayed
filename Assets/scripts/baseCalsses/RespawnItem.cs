using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnItem : MonoBehaviour
{
    public float minRespawnTime = 10.0f;
    public float maxRespawnTime = 60.0f;
    public float currentRespawnTime;
    public bool deactivated;
    public float timeDeactivated;
    public Vector3 spawnLocation;
    public void Start()
    {
        timeDeactivated = 0.0f;
        deactivated = !gameObject.activeSelf;
        timeDeactivated = 0.0f;
        currentRespawnTime = 0.0f;
        spawnLocation = transform.position;
    }

    void Update()
    {

    }
}
