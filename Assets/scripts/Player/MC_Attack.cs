using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Attack : AttackBase
{
    public float GetLastDrawTime() => drawTime;
        

    [SerializeField] private float comboWindowTime = 0.5f; // Time to trigger a combo attack
    [SerializeField] private float clickBuffer = 0.25f; // Prevent button mashing
    private float lastAttackTime;
    private bool attackQueued;

    private float drawTime = -1.0f;
    //private float attackTime = -1.0f;
    private const float drawBuffer = .3f;


    override
    public bool IsAttacking() => currentState == AttackState.Attacking;
    protected AttackState currentState { get; private set; }
    protected enum AttackState
    {
        Idle,
        Attacking,
        Combo
    };

    void Start()
    {
        currentState = AttackState.Idle;
        lastAttackTime = -comboWindowTime; // Ensure first attack can trigger
        attackQueued = false;
    }

    override public bool AttackTrigger()
    {
        return Input.GetMouseButtonDown(0);
    }

    override public void Attack()
    {
        float currentTime = Time.time;

        MC_EquippedWeapon mcWeapon = equippedWeaponController as MC_EquippedWeapon;

        // Ignore attacks if clicking too fast
        if (currentTime - lastAttackTime < clickBuffer)
        {
            Debug.Log("Clicked too soon!");
            return;
        }

        if(equippedWeaponController.hasWeaponEquipped() && 
            currentTime - drawTime > drawBuffer && 
            currentTime - mcWeapon.GetSheathingTime() > 0.5f
            )
        {
            drawTime = Time.time;
            if (equippedWeaponController.isDrawn())
            {
                if (currentState == AttackState.Idle)
                {
                    StartAttack();
                }
                else if (currentState == AttackState.Attacking && (currentTime - lastAttackTime <= comboWindowTime))
                {
                    attackQueued = true;
                    Debug.Log("Attack queued for combo!");
                }
            }
            else
            {
                Debug.Log("Weapon not Drawn");
                equippedWeaponController.DrawWeapon(true);
                
            }
        }
        else
        {
            return;
        }
    }

    private void StartAttack()
    {
        Debug.Log("Starting attack!");
        lastAttackTime = Time.time;
        animationManager.SetBool("isAttacking", true);
        currentState = AttackState.Attacking;
    }

    // Called by an Animation Event at the end of an attack animation
    public void AttackEnd()
    {
        Debug.Log("Attack ended. Current state: " + currentState);

        if (attackQueued)
        {
            Debug.Log("Executing queued attack!");
            attackQueued = false;
            currentState = AttackState.Combo;
            StartAttack();
        }
        else
        {
            Debug.Log("Returning to idle.");
            currentState = AttackState.Idle;
            animationManager.SetBool("isAttacking", false); // Reset the attacking flag
        }
    }


}