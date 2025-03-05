using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    // Enum to track the state of the weapon (now public)
    public enum State
    {
        Dropped,    // Default state in open world
        Inventory,  // Obtained by player but not equipped
        Equipped,   // Equipped in sheath
        Drawn       // In hand, actively being used
    }
    
    public GameObject Weapon;
    public GameObject Sheath;
    public string WeaponName;
    [SerializeField] string weaponType;
    public float attackPower;
    public float damage;

    public string DrawAnimation;
    public string SheathAnimation;


    public bool twoHanded;
    public string ActionBoneRequired; // To be set in prefab
    public string ActionBoneTwoRequired; // To be set in prefab
    public string SheathBoneRequired; // to be set in prefab

    public Transform ActionBoneR;  // Bone for when the weapon is drawn (e.g., Right hand)
    public Transform ActionBoneL;  // "..." Left Hand
    public Transform SheathedBone; // Bone for when the weapon is sheathed (e.g., hip or back)
    public Vector3 actionBoneLocalRotationAdjustments;

    private EquippedWeaponBase user; // reference to the EquippedWeapon of the user

    private State currentState = State.Dropped;

    // Expose public constants for easy state reference
    public static readonly State DroppedState = State.Dropped;
    public static readonly State InventoryState = State.Inventory;
    public static readonly State EquippedState = State.Equipped;
    public static readonly State DrawnState = State.Drawn;



    // Get the current state of the weapon
    public State GetWeaponState()
    {
        return currentState;
    }

    public void SetUserData(EquippedWeaponBase equippedWeapon)
    {
        user = equippedWeapon;
        if(user != null)
        {
            Dictionary<string, Transform> boneData = user.GetWeaponBoneData;
            ActionBoneR = boneData[ActionBoneRequired];
            ActionBoneL = boneData[ActionBoneTwoRequired];
            SheathedBone = boneData[SheathBoneRequired];
        }
        else
        {
            Debug.LogError("User is null");
        }

    }

    // Set the weapon state and handle the transition logic
    public void SetWeaponState(State nextState)
    {
        currentState = nextState;

        // Handle different states
        switch (nextState)
        {
            case State.Inventory:
                // Should not be rendered in the scene when in inventory
                if (Weapon != null) Weapon.SetActive(false);
                if (Sheath != null) Sheath.SetActive(false);
                break;

            case State.Equipped:
                // Should be equipped (shown in sheath)
                if (Weapon != null) Weapon.SetActive(true);
                if (Sheath != null) Sheath.SetActive(true);
                // Add additional logic if necessary for when the weapon is equipped
                break;

            case State.Drawn:
                // Should be drawn and ready for use (attached to hand)
                if (Weapon != null) Weapon.SetActive(true);
                if (Sheath != null) Sheath.SetActive(true);
                // Additional logic for positioning the weapon, attaching to the action bone, etc.
                break;

            case State.Dropped:
                // Handle dropped state logic (e.g., laying on the ground)
                if (Weapon != null) Weapon.SetActive(true);  // Keep weapon active if dropped
                if (Sheath != null) Sheath.SetActive(false);  // No need to show the sheath
                break;

            default:
                Debug.LogWarning("Unknown weapon state");
                break;
        }
    }
}
