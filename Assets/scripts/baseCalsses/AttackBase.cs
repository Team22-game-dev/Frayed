using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour, IAttack
{
    protected EquippedWeaponBase equippedWeaponController;

    protected AnimationManager animationManager;


    public void Awake()
    {
        //Debug.Log("Attack Base Awake!");
        animationManager = GetComponent<AnimationManager>();
        equippedWeaponController = GetComponent<EquippedWeaponBase>();

        if (animationManager == null)
            Debug.LogError("AnimationManager component not found from AttackBase");

        if (equippedWeaponController == null)
            Debug.LogError("EquippedWeaponBase component not found from AttackBase");
    }

    // Update is called once per frame
    protected void Update()
    {
        // check for attack input based on derived class.
        if(AttackTrigger())
            Attack();
    }

    public abstract bool AttackTrigger();

    public abstract void Attack();

    public abstract bool IsAttacking();

}
