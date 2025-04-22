using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TakeDamageBase : MonoBehaviour
{

    private bool inFire = false;

    [SerializeField]
    private float fireDamageInterval = 1.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapons"))
        {
           if(other.gameObject.layer == LayerMask.NameToLayer("Fire") && !inFire)
           {
                inFire = true;
                StartCoroutine(DelayFireDamage());
           }
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

        if (attackSM.IsAttacking())
        {
            HandleDamage(attacker, attackingWeapon);
        }
        // TODO: Just some dummy logic to make oil barrel hitboxes better....
        else if (GetComponent<EnemyData>() != null && GetComponent<EnemyData>().enemyName == "Oil Barrel")
        {
            StartCoroutine(PollAttacking(attackSM, attacker, attackingWeapon));
        }
    }

    void OnTriggerExit(Collider other)
    {
        // TODO: This assumes there can only be one fire.
        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
        {
            inFire = false;
        }
    }

    private IEnumerator PollAttacking(IAttack attackSM, GameObject attacker, GameObject attackingWeapon)
    {
        float startTime = Time.time;
        while (Time.time - startTime <= 0.5f)
        {
            if (attackSM.IsAttacking())
            {
                HandleDamage(attacker, attackingWeapon);
                yield break;
            }
            yield return null;
        }
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

    private IEnumerator DelayFireDamage()
    {
        while(inFire)
        {
            yield return new WaitForSeconds(fireDamageInterval);
            if(inFire)
                FireDamage();
        }
    }


    public abstract void HandleDamage(GameObject attacker, GameObject attackingWeapon);

    public abstract void FireDamage();
    
}
