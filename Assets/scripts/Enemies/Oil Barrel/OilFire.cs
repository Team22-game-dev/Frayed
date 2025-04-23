using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilFire : MonoBehaviour
{
    private EnemyData enemyData;

    [SerializeField]
    private GameObject oilParticlePrefab;

    [SerializeField]
    private GameObject brokenBarrelPrefab;
    
    void Start()
    {
        enemyData = GetComponent<EnemyData>();
        Debug.Assert(enemyData != null);


    }

    void Update()
    {

        if (Mathf.Approximately(enemyData.GetHealthRatio(), 0.0f))
        {
            ActivateFire();
        }
        
    }

    private void ActivateFire()
    {

        GameObject oilSprayer = Instantiate(oilParticlePrefab, transform.position, transform.rotation);
        GameObject brokenBarrel = Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

        if(oilSprayer != null)
        {
            Destroy(oilSprayer, 0.8f);
        }
    }
}
