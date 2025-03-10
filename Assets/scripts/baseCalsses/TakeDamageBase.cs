using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TakeDameageBase : MonoBehaviour
{

    GameObject AttackingWeapon = null;
    void OnTriggerEnter(Collider other)
    {
        // Check if the object the weapon entered is on the Weapons layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapons"))
        {
            // Handle the trigger with another object on the Weapons layer
            Debug.Log("Weapon entered " + gameObject.name + "'s collader from the Weapons layer!");
            AttackingWeapon = other.gameObject;
            Debug.Log("Attacking Weapon: " + other.gameObject.name);
            HandleDamage();
        }
    }

    public abstract void HandleDamage();
    
}
