using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmalgamAttack : EnemyAttack
{
    override
    //public bool IsAttacking() => currentState == AttackState.Attacking;

    public bool IsAttacking()
    {
        //Debug.Log("Amalgam is attacking " + (currentState == AttackState.Attacking) + " current state: " + currentState);
        return (currentState == AttackState.Attacking);
    }
    public enum AttackState
    {
        Idle,
        PreparingAttack,
        Transition,
        Attacking,
        Recovering
    }

    public AttackState currentState { get; private set; } = AttackState.Idle;

    [SerializeField] private GameObject amalgamSwordPrefab;
    private GameObject SwordInstance;

    private Transform handBoneR, handBoneL;

    private SphereCollider clawSphereLeftHand;
    private SphereCollider clawSphereRightHand;

    override public void Attack()
    {
        if (currentState != AttackState.Idle) return;

        if(animationManager.GetBool("Sword"))
        {
            swordAttack();
        }
        else if (enemyManager.enemyData.GetHealthRatio() < .50f)
        {
            clawSphereLeftHand.enabled = false;
            clawSphereRightHand.enabled = false;

            if (!equippedWeaponController.isDrawn() && animationManager.GetCurrentAnimationName() != "DrawSword")
            {
                StartCoroutine(EquipSword());  
            }
        }
        else
        {
            clawAttack();
        }
    }

    private void clawAttack()
    {
        if (currentState != AttackState.Idle) return;

        currentState = AttackState.PreparingAttack;

        Debug.Log("Claw Attack!");
        if (clawSphereLeftHand == null || clawSphereRightHand == null)
        {
            Debug.Log("ClawSpheres null");
            if (handBoneL == null || handBoneR == null)
            {
                Debug.LogError("Hand bones null on Amalgam!");
                currentState = AttackState.Idle;
                return;
            }
            return;
        }

        animationManager.SetTrigger("Attack");
        currentState = AttackState.Attacking;
        StartCoroutine(ResetStateAfterDelay(1f)); // or however long the animation is
    }

    private void swordAttack()
    {
        currentState = AttackState.PreparingAttack;

            if (!equippedWeaponController.hasWeaponEquipped())
            {
                Debug.LogWarning("Amalgam Sword not equipped!");
                return;
            }
            else
            {
                currentState = AttackState.Attacking;
                animationManager.SetTrigger("Attack");
                StartCoroutine(ResetStateAfterDelay(0.893f)); // length of the sword attack anim
            }
    }



    public void InstantiateSword()
    {
        //Vector3 centerOfEnemy = transform.position;
        SwordInstance = Instantiate(amalgamSwordPrefab, gameObject.transform.position, Quaternion.identity);
    }

    new void Start()
    {
        base.Start();
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForEndOfFrame();

        if (equippedWeaponController != null)
        {
            Dictionary<string, Transform> boneData = equippedWeaponController.GetWeaponBoneData;
            if (boneData != null && boneData.Count > 0)
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

    private IEnumerator ResetStateAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        currentState = AttackState.Idle;
    }

    private IEnumerator EquipSword()
    {
        var start = Time.time;
        GameObject sword = Instantiate(amalgamSwordPrefab);
        

        yield return null; // wait a frame
        
        WeaponData weaponData = sword.GetComponent<WeaponData>();
        weaponData.PrepForUse();

        yield return null;

        yield return equippedWeaponController.StartEquipWeaponCoroutine(sword);

        //equippedWeaponController.DrawWeapon(false);
        yield return null;
        //animationManager.SetBool("Sword", true);
        Debug.Log("useSword: " + animationManager.GetBool("Sword"));

    }
}
