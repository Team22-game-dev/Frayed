using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MC_EquippedWeapon : EquippedWeaponBase
{

    [SerializeField] private KeyCode pickupKey = KeyCode.E; // key to pick up weapon

    // use singleton since only one weapon should be equipped at a time
    private static MC_EquippedWeapon _instance; // the local private _instance
    public static MC_EquippedWeapon Instance => _instance;

    private MC_Attack mc_AttackController;

    private float sheathingTime = -1.0f;

    new private void Awake()
    {
        base.Awake();

        mc_AttackController = GetComponent<MC_Attack>();

        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        
        if(isDrawn() && WillSheathWeapon())
        {
            StartCoroutine(SheathWeapon());
        }
    }

    // from Base Class
    override public bool WillPickupWeapon() // If player would like to pick up the weapon they should press E
    {
        return Input.GetKeyDown(pickupKey);
    }

    private bool WillSheathWeapon()
    {
        return Input.GetMouseButtonDown(1);
    }

    public IEnumerator SheathWeapon()
    {
        if(Time.time - mc_AttackController.GetLastDrawTime() > 0.3f)
        {
            sheathingTime = Time.time;
            float start = Time.time;
            while (Time.time - start <= 0.5f)
            {
                if (animationManager.GetCurrentAnimationName() == "encounter_idle")
                {
                    if (weaponData != null && currentWeaponState == WeaponState.Drawn)
                    {
                        animationManager.SetTrigger(weaponData.SheathAnimation);
                        yield break;
                    }
                    else
                    {
                        Debug.LogError("weaponData null or weapon in wrong state");
                        yield break;
                    }
                }
                else
                {
                    string animationName = animationManager.GetCurrentAnimationName();
                    if (animationName == "jogg_w_dagger")
                    {
                        animationManager.SetTrigger("SheathDagger");
                        SheathAndDrawWeapon();
                        yield break;
                    }
                }

                yield return null; // check again next frame
            }

            Debug.Log("sheath time out");
        }
        else
        {
            Debug.Log("sheath blocked by draw time buffer");
        }
    }


    public float GetSheathingTime()
    {
        return sheathingTime;
    }


}
