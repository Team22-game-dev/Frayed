using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_TakeDamageController : TakeDamageBase
{
    override
    public void HandleDamage(GameObject attacker, GameObject attackingWeapon)
    {
        Debug.Log("MC handle damage");
        if(attacker == gameObject)
        {
            Debug.Log("MC self weapon collision");
            return;
        }
            
        EquippedWeaponBase attackerEquippedWeapon = attacker.GetComponent<EquippedWeaponBase>();
        EnemyData enemyData = attacker.GetComponent<EnemyData>();

        if(attackerEquippedWeapon != null)
        {
            if(attackerEquippedWeapon.hasWeaponEquipped())
            {
                Debug.Log("Attacking with weapon");
                float damage = 0.0f;

                var attackerData = attacker.GetComponent<IAttackData>();
                

                if(attackerData == null)
                {
                    Debug.LogError("Missing required Components on attacker or MC");
                    return;
                }

                if(attackerEquippedWeapon != null && attackerEquippedWeapon.isDrawn())
                {
                    var weaponData = attackerEquippedWeapon.GetWeaponData();
                    if(weaponData != null)
                    {
                        damage = Mathf.Round((attackerData.GetAttackPower() + weaponData.GetWeaponPower()) / 5f / 0.25f) * 0.25f;
                    }
                }
                Debug.Log("DamageDealt: " + damage);
                PlayerStats.Instance.TakeDamage(damage);
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

    override
    public void FireDamage()
    {
        PlayerStats.Instance.TakeDamage(0.25f);
    }
}
