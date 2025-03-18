using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_TakeDamageController : TakeDameageBase
{
    override
    public void HandleDamage(GameObject attacker, GameObject attackingWeapon)
    {
        EquippedWeaponBase attackerEquippedWeapon = attacker.GetComponent<EquippedWeaponBase>();
        EnemyData enemyData = attacker.GetComponent<EnemyData>();

        if(attackerEquippedWeapon != null)
        {
            if(attackerEquippedWeapon.hasWeaponEquipped())
            {
                Debug.Log("Attacking with weapon");


            }
            else
            {
                Debug.Log("Attacking with hands");
                float damageDealt = enemyData.GetAttackPower();

                Debug.Log("DamageDealt: " + damageDealt);
                PlayerStats.Instance.TakeDamage(damageDealt);
            }
        }
        else
        {
            Debug.LogError("couldnot get attackers Equipped weapon component!");
        }
    }
}
