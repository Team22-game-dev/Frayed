using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TakeDamageBase : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapons"))
        {
           // Debug.Log("Collided with: " + other.name);
            return;
        }

        //Debug.Log($"Weapon {other.name} entered {gameObject.name}'s collider from the Weapons layer!");

        GameObject attackingWeapon = other.gameObject;
        GameObject attacker = GetAttacker(attackingWeapon);

        if (attacker == null)
        {
            Debug.Log("Random weapon collider: " + attackingWeapon.name);
            return;
        }


        IAttack attackSM = attacker.GetComponent<IAttack>();
        if (attackSM == null)
        {
            Debug.LogError("IAttack not found");
        }
        if(!attackSM.IsAttacking())
        {
           // Debug.Log("Wasn't trying to attack");
            return;
        }

        //Debug.Log("Attacking Weapon: " + attackingWeapon.name);
        HandleDamage(attacker, attackingWeapon);
    }


    private GameObject GetAttacker(GameObject attackingWeapon)
    {
        if (attackingWeapon != null)
        {

            var weaponData = attackingWeapon.GetComponent<WeaponData>();
            if(weaponData != null)
            {
                var attacker = weaponData.GetOwner();
                if(attacker != null)
                    return attacker;
            }

            Transform root = attackingWeapon.transform;

            while (root.parent != null)
            {
                //Debug.Log("layer: " + root.gameObject.layer);
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
