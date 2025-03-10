using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmalgamAttack : EnemyAttack
{
    [SerializeField]
    GameObject amalgamSwordPrefab;
    private GameObject SwordInstance;

    private Transform handBoneR, handBoneL;
    
    EquippedWeaponBase equippedWeapon;

    private SphereCollider clawSphereLeftHand;
    private SphereCollider clawSphereRightHand;

    override public void Attack(){
        Debug.Log("Amalgam Attacks!");

        if(enemyManager.enemyData.GetHealthRatio() < .51)
        {
            clawSphereLeftHand.enabled = false;
            clawSphereRightHand.enabled = false;


            if(!equippedWeapon.isDrawn())
            {   
                
                animationManager.SetTrigger("DrawSword");
            }

            StartCoroutine(swordAttack());
        }
        else
        {
            clawAttack();
        }
        
    }

    private void clawAttack()
    {
        // Check if hand sphered created if not instrnatiate them to collide to cause damage
        if(clawSphereLeftHand == null || clawSphereRightHand == null){
            if(handBoneL == null || handBoneR == null)
            {
                Debug.LogError("Hand bones null on Amalgam!");
                return;
            }

        }

        animationManager.SetTrigger("ClawAttack");
    }

    private IEnumerator swordAttack()
    {
        int trys = 0;
        do{
            if(!equippedWeapon.hasWeaponEquipped())
            {
                
                
                Debug.LogWarning("Amalgam Sword not equipped!");

                if(trys < 4)
                {
                     yield return new WaitForSeconds(.33f);
                }
                else
                {
                    Debug.LogError("Equip Amalgam Sword failed");
                     yield break;
                }
            
            }
            else
            {
                animationManager.SetTrigger("SwordAttack");
                break; // break while loop
            }
        }while(true);
        
    }


    public void AttackInterrupt()
    {
        if(enemyManager.currentState != EnemyManager.State.READY_TO_ATTACK)
            animationManager.SetTrigger("InterruptAttack");
    }
    public void InstantiateSword()
    {
        SwordInstance = Instantiate(amalgamSwordPrefab, handBoneR.position, handBoneR.rotation);
    }


    void Start()
    {
        base.Start();

        clawSphereLeftHand = handBoneL.gameObject.AddComponent<SphereCollider>();
        clawSphereRightHand = handBoneR.gameObject.AddComponent<SphereCollider>();
        clawSphereLeftHand.isTrigger = clawSphereRightHand.isTrigger = true;
        clawSphereLeftHand.radius = clawSphereRightHand.radius = 0.14f;

        // Accessing the dictionary via the property
        var boneData = equippedWeapon.GetWeaponBoneData;

        // Accessing specific hand bones using keys
        if (boneData.TryGetValue("WeaponHandFowardL", out handBoneL) && 
            boneData.TryGetValue("WeaponHandFowardR", out handBoneR))
        {
            // Both bones found successfully
            Debug.Log("Hand bones successfully assigned.");
        }
        else
        {
            // One or both bones are null or not found
            Debug.LogError("Hand bones are null or not found.");
        }
    }

}
