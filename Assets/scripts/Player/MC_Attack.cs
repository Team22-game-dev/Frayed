using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Attack : AttackBase
{
    public float GetLastDrawTime() => drawTime;
        

    [SerializeField] private float comboWindowTime = 0.5f; // Time to trigger a combo attack
    [SerializeField] private float clickBuffer = 0.25f; // Prevent button mashing
    private float lastAttackTime;
    //private bool attackQueued;

    private float drawTime = -1.0f;
    //private float attackTime = -1.0f;
    private const float drawBuffer = .3f;

    private int currentAttack = 0;
    private int nextAttack = 0;


    override
    public bool IsAttacking() => currentState == AttackState.Attacking;
    protected AttackState currentState { get; private set; }
    protected enum AttackState
    {
        Idle,
        Attacking,
        //Combo
    };

    void Start()
    {
        currentState = AttackState.Idle;
        lastAttackTime = -comboWindowTime; // Ensure first attack can trigger
        //attackQueued = false;
    }

    new
    void Update()
    {
        base.Update(); // run abstract Update process
        if(Time.time - lastAttackTime > 0.6f)
        {
            currentAttack = nextAttack = 0; // reset attacks sequence if pause between attack requests 
            currentState = AttackState.Idle;
            animationManager.ResetTrigger("Attack");
            animationManager.SetInt("attackNumber", nextAttack);
        }
        
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
                    Debug.Log("Still attakcing, wait for animation event");
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
        animationManager.SetTrigger("Attack");


        WeaponData weaponData = equippedWeaponController.GetWeaponData();

        if(weaponData == null)
        {
            Debug.LogError("Weapon data is null when trying tun increment attack");
        }

        nextAttack = (currentAttack + 1) % weaponData.GetNumAttacks();
        currentState = AttackState.Attacking;
    }

    // Called by an Animation Event at the end of an attack animation
    public void AttackEnd()
    {
        
        animationManager.SetInt("attackNumber", nextAttack);
        currentAttack = nextAttack;
        currentState = AttackState.Idle;
        
    }


}