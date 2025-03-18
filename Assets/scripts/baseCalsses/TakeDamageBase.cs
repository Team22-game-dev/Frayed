using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TakeDameageBase : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // Check if the object the weapon entered is on the Weapons layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapons"))
        {
            GameObject attackingWeapon = other.gameObject;
            // Handle the trigger with another object on the Weapons layer
            Debug.Log("Weapon entered " + gameObject.name + "'s collider from the Weapons layer!");
            GameObject attacker = GetAttacker(attackingWeapon);
            if(attacker != null)
            {
                Debug.Log("Attacking Weapon: " + other.gameObject.name);
                HandleDamage(attacker, attackingWeapon);
            }
            else
            {
                Debug.Log("Random weapon collider");
            }
            
        }
    }

    private GameObject GetAttacker(GameObject attackingWeapon)
    {
        if (attackingWeapon != null)
        {
            Transform root = attackingWeapon.transform;

            while (root.parent != null)
            {
                Debug.Log("layer: " + root.gameObject.layer);
                root = root.parent;
            }

            if (root.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                return root.gameObject;
            }
        }

        return null;
    }


    public abstract void HandleDamage(GameObject attacker, GameObject attackingWeapon);
    
}
