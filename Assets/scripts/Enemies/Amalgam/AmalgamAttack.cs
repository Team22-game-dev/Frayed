using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmalgamAttack : EnemyAttack
{
    [SerializeField]
    GameObject amalgamSwordPrefab;
    private GameObject SwordInstance;

    private Transform handBoneR, handBoneL;
    
    private SphereCollider clawSphereLeftHand;
    private SphereCollider clawSphereRightHand;

    override public void Attack(){
        Debug.Log("Amalgam Attacks!");

        if(enemyManager.enemyData.GetHealthRatio() < .51)
        {
            clawSphereLeftHand.enabled = false;
            clawSphereRightHand.enabled = false;


            if(!equippedWeaponController.isDrawn())
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
        Debug.Log("Claw Attack!");
        // Check if hand sphered created if not instrnatiate them to collide to cause damage
        if(clawSphereLeftHand == null || clawSphereRightHand == null){
            Debug.Log("ClawSpheres null");
            if(handBoneL == null || handBoneR == null)
            {
                Debug.LogError("Hand bones null on Amalgam!");
                return;
            }
            return;
        }

        animationManager.SetTrigger("ClawAttack");
    }

    private IEnumerator swordAttack()
    {
        int trys = 0;
        do{
            if(!equippedWeaponController.hasWeaponEquipped())
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



    new void Start()
    {
        Debug.Log("AmalgamAttack Start");
        Debug.Log("Addign colliders to " + gameObject.name);
        base.Start();

        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForEndOfFrame(); // Ensures all Start() calls have executed

        if (equippedWeaponController != null)
        {
            Dictionary<string, Transform> boneData = equippedWeaponController.GetWeaponBoneData;
            if (boneData != null && boneData.Count > 0) // Ensure dictionary isn't empty
            {
                handBoneL = boneData["WeaponHandFowardL"];
                handBoneR = boneData["WeaponHandFowardR"];
            }
            else
            {
                Debug.LogError("BoneData dictionary is empty in AmalgamAttack!");
            }
        }
        else
        {
            Debug.LogError("equippedWeaponController is null in AmalgamAttack!");
        }

        if (handBoneL != null && handBoneR != null)
        {
            clawSphereLeftHand = handBoneL.gameObject.AddComponent<SphereCollider>();
            clawSphereRightHand = handBoneR.gameObject.AddComponent<SphereCollider>();
            clawSphereLeftHand.isTrigger = clawSphereRightHand.isTrigger = true;
            clawSphereLeftHand.radius = clawSphereRightHand.radius = 0.14f;
            clawSphereLeftHand.enabled = clawSphereRightHand.enabled = true;
            clawSphereLeftHand.gameObject.layer = clawSphereRightHand.gameObject.layer = LayerMask.NameToLayer("Weapons");
        }
        else
        {
            Debug.LogError("Hand bones are null in AmalgamAttack!");
        }
    }


}
