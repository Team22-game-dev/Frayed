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

    private GameObject oilSprayer;
    private GameObject brokenBarrel;
    private GameObject woodenBarrel;

    public bool flamable;
    public bool onFire;
    public bool activated;
    
    void Start()
    {
        enemyData = GetComponent<EnemyData>();
        Debug.Assert(enemyData != null);

        woodenBarrel = transform.Find("wooden_barrel").gameObject;
        Debug.Assert(woodenBarrel != null);

        onFire = false;
        flamable = false;
        activated = false;
    }

    void Update()
    {

        if (Mathf.Approximately(enemyData.GetHealthRatio(), 0.0f) && !activated)
        {
            ActivateFire();
        }

        if (flamable)
        {
            Debug.Assert(oilSprayer != null);
            OilSplashHandler oilSplashHandler = oilSprayer.gameObject.GetComponentInChildren<OilSplashHandler>();
            if (oilSplashHandler != null && oilSplashHandler.oilDecals != null)
            {
                foreach (GameObject oilDisk in oilSplashHandler.oilDecals)
                {
                    if (oilDisk == null)
                    {
                        continue;
                    }
                    FireController fireController = oilDisk.GetComponent<FireController>();
                    if (fireController.onFire)
                    {
                        flamable = false;
                        onFire = true;
                    }
                }
            }
            if (brokenBarrel != null)
            {
                FireController fireController = brokenBarrel.GetComponentInChildren<FireController>();
                if (fireController != null)
                {
                    if (fireController.onFire)
                    {
                        flamable = false;
                        onFire = true;
                    }
                }
            }
        }
        else if (onFire)
        {
            Debug.Assert(oilSprayer != null);
            bool tempOnFire = false;
            OilSplashHandler oilSplashHandler = oilSprayer.gameObject.GetComponentInChildren<OilSplashHandler>();
            if (oilSplashHandler != null && oilSplashHandler.oilDecals != null)
            {
                foreach (GameObject oilDisk in oilSplashHandler.oilDecals)
                {
                    if (oilDisk == null)
                    {
                        continue;
                    }
                    FireController fireController = oilDisk.GetComponent<FireController>();
                    if (fireController.onFire)
                    {
                        tempOnFire = true;
                    }
                }
            }
            if (brokenBarrel != null)
            {
                FireController fireController = brokenBarrel.GetComponentInChildren<FireController>();
                if (fireController != null)
                {
                    if (fireController.onFire)
                    {
                        tempOnFire = true;
                    }
                }
            }
            onFire = tempOnFire;
            if (!onFire)
            {
                gameObject.SetActive(false);
            }
        }
        
    }

    private void ActivateFire()
    {
        MeshRenderer meshRender = woodenBarrel.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            meshRender.enabled = false;
        }
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        oilSprayer = Instantiate(oilParticlePrefab, transform.position, transform.rotation);
        brokenBarrel = Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

        activated = true;
        flamable = true;
        if (oilSprayer != null)
        {
            StartCoroutine(DeactivateOilSprayer(0.8f));
            //Destroy(oilSprayer, 0.8f);
        }
    }

    private IEnumerator DeactivateOilSprayer(float sec)
    {
        yield return new WaitForSeconds(sec);
        oilSprayer.SetActive(false);
    }
}
