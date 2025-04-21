using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;


public class MC_EquippedWeapon : EquippedWeaponBase
{
    // use singleton since only one weapon should be equipped at a time
    private static MC_EquippedWeapon _instance; // the local private _instance
    public static MC_EquippedWeapon Instance => _instance;

    private MC_Attack mc_AttackController;

    private float sheathingTime = -1.0f;

    private InputManager inputManager;

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

    new public void Start()
    {
        base.Start();
        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);
    }

    private void Update()
    {
        
        if(isDrawn() && WillSheathWeapon())
        {
            StartCoroutine(SheathWeapon());
        }
    }

    private void LateUpdate()
    {
        inputManager.pickupWeapon = false;
        inputManager.sheathWeapon = false;
    }

    // from Base Class
    override public bool WillPickupWeapon() // If player would like to pick up the weapon they should press E
    {
        return inputManager.pickupWeapon;
    }

    private bool WillSheathWeapon()
    {
        return inputManager.sheathWeapon;
    }

    public IEnumerator SheathWeapon()
    {
        if(Time.time - mc_AttackController.GetLastDrawTime() > 0.3f)
        {
            Debug.Log("Sheathing weapon");
            sheathingTime = Time.time;
            float start = Time.time;
            // while (Time.time - start <= 0.5f)
            // {
                
            //     // if (animationManager.GetCurrentAnimationName() == "encounter_idle")
            //     // {
            //     //     if (weaponData != null && currentWeaponState == WeaponState.Drawn)
            //     //     {
            //     //         SheathAndDrawWeapon();
            //     //         yield break;
            //     //     }
            //     //     else
            //     //     {
            //     //         Debug.LogError("weaponData null or weapon in wrong state");
            //     //         yield break;
            //     //     }
            //     // }
            //     // else
            //     // {
            //     //     string animationName = animationManager.GetCurrentAnimationName();
            //     //     if (animationName == "jogg_w_dagger")
            //     //     {
            //     //         //animationManager.SetTrigger("SheathDagger");
            //     //         SheathAndDrawWeapon();
            //     //         yield break;
            //     //     }
            //     // }

               
            // }
            yield return null;
            SheathAndDrawWeapon();

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

    public void ResetPlayerAnimation()
    {
        // TODO: Probably need to improve on this, but we to make sure weapons are unequipped.
        animationManager.ResetTrigger("Attack");
        //animationManager.ResetTrigger("Sheath");
        animationManager.SetTrigger("Unequip");
        StartCoroutine(DelayedForceResetUnequip());
        if (weaponData != null)
        {
            StartCoroutine(DelayedBoolOff(weaponData.GetWeaponType()));
        }
    }

    public IEnumerator DelayedForceResetUnequip()
    {
        //yield return null; // wait one frame
        yield return new WaitForSeconds(0.5f); // Wait a little for the unequip to fully kick in.
        animationManager.ResetTrigger("Unequip");
    }


}
