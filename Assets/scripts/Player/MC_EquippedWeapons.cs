using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MC_EquippedWeapon : EquippedWeaponBase
{

    [SerializeField] private KeyCode pickupKey = KeyCode.E; // key to pick up weapon

    // use singleton since only one weapon should be equipped at a time
    private static MC_EquippedWeapon _instance; // the local private _instance
    public static MC_EquippedWeapon Instance => _instance;

    new private void Awake()
    {
        base.Awake();
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
            SheathWeapon();
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

    public void SheathWeapon()
    {
        Debug.Log("Sheathing weapon");
        if(animationManager.GetCurrentAnimationName() == "encounter_idle")
        {
            // play sheathing Animation animation
            if(weaponData != null && currentWeaponState == WeaponState.Drawn)
            {
                animationManager.SetTrigger(weaponData.SheathAnimation);
            }
            else
            {
                Debug.LogError("weaponData null or weapon in wrong state");
            }
        }
        else
        {
            // dont play sheatingn animation
            SheathAndDrawWeapon();
        }
    }



}
