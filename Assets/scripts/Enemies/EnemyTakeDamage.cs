using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : TakeDamageBase
{
    public override void HandleDamage(GameObject attacker, GameObject attackingWeapon)
    {
        if(attacker == gameObject)
        {
            //Debug.Log("collision with self! " + attacker.name + " weapon: " + attackingWeapon.name);
            return;
        }
        Debug.LogWarning("Enemy Taking Damage! " + attacker.name + " with " + attackingWeapon.name);
        var attackerData = attacker.GetComponent<IAttackData>(); 
        var equippedWeaponBase = attacker.GetComponent<EquippedWeaponBase>();
        var enemyData = GetComponent<EnemyData>(); // Note: class names are PascalCase in C#

        float damage = 0.0f;

        if (attackerData == null || enemyData == null)
        {
            Debug.LogWarning("Missing required components on attacker or enemy.");
            return;
        }

        if (equippedWeaponBase != null && equippedWeaponBase.isDrawn())
        {
            var weaponData = equippedWeaponBase.GetWeaponData();

            if (weaponData != null)
            {
                damage = attackerData.GetAttackPower() + weaponData.GetWeaponPower();
            }
            else
            {
                Debug.LogWarning("Weapon data not found or missing IWeaponData component.");
                damage = attackerData.GetAttackPower();
            }
        }
        else
        {
            damage = attackerData.GetAttackPower();
        }

        enemyData.TakeDamage(damage);
        DamageIndicator.Instance.IndicateDamage(damage, attackingWeapon.transform.position);
    }
}
