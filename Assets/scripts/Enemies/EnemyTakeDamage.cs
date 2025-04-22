using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : TakeDamageBase
{

    [SerializeField]
    private float elementalDamage = 100;
    public override void HandleDamage(GameObject attacker, GameObject attackingWeapon)
    {
        if(attacker == gameObject)
        {
            //Debug.Log("collision with self! " + attacker.name + " weapon: " + attackingWeapon.name);
            return;
        }
       // Debug.LogWarning("Enemy Taking Damage! " + attacker.name + " with " + attackingWeapon.name);
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

        // TODO: Probably a better place to put this, but this will do for now.
        if (attackerData.GetType() == typeof(EnemyData) && ((EnemyData)attackerData).enemyName == "Oil Barrel")
        {
            damage *= 10.0f;
        }

        enemyData.TakeDamage(damage);
        if (attacker.gameObject.CompareTag("Player"))
        {
            DamageIndicator.Instance.IndicateDamage(damage, attackingWeapon.transform.position);
        }
    }

    override
    public void FireDamage()
    {
        Debug.Log("Enemy Taking fire damage");
        
        var enemyData = GetComponent<EnemyData>();

        enemyData.TakeDamage(elementalDamage);


    }
}
