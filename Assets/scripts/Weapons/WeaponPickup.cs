using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Describes pickup behavior
 * Weapons can be picked up by all of the player, enemies and other NPCs
 */

public class WeaponPickup : MonoBehaviour
{

    [SerializeField] private WeaponData weaponData; // Reference to the weapon's data
    [SerializeField] private float pickupRadius = 1.5f; // radius from weapon in which the weapon can be picked up

    private GameObject PotentialUser = null;
    private SphereCollider sphereCollider;
    private Rigidbody rigidBody;
    [SerializeField] private EquippedWeaponBase userWeaponController;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        // add sphere collider to weapon object for its pickup radius
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = pickupRadius;
        sphereCollider.enabled = true;

        // Set collider to weapon pickup layer
        gameObject.layer = LayerMask.NameToLayer("Weapons");
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in rang and would like to pickup the weapon
        if((PotentialUser != null) && PickupTrigger())
        {
            PickupWeapon();
        }
        
    }

    private bool PickupTrigger()
    {

        if(userWeaponController != null)
        {
            return(userWeaponController.willPickupWeapon());
        }
        else
        {
            Debug.Log("UserWwaponController null when checking for Pickup Trigger");
            return false;
        }

                // if enemy or NPC, request if they would like to pick up the weapon whose answer will be context specific
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider Triggered");

        userWeaponController = other.GetComponent<EquippedWeaponBase>();

        if (userWeaponController != null)
        {
            PotentialUser = other.gameObject;
            Debug.Log($"{other.gameObject.name} is in range to pick up the weapon.");
        }
    }





private void OnTriggerExit(Collider other)
{
    if (other.gameObject == PotentialUser)
    {
        PotentialUser = null;
        userWeaponController = null;
        Debug.Log("Player has left range to pickup weapon.");
    }
}

    private void PickupWeapon()
    {
        Debug.Log("Picking Up Weapon!");
        sphereCollider.enabled = false; // Disable sphere collider
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;


        if (userWeaponController == null)
        {
            Debug.Log("Tried to equip to null user");
            return;
        }

        if (userWeaponController.hasWeaponEquipped())
        {
            Debug.Log("To Inventory");
            // TODO: Send to inventory
        }
        else
        {
            // Pass the weapon instance (gameObject) to the equip function
            userWeaponController.StartEquipWeaponCoroutine(gameObject);
            userWeaponController.DrawWeapon(false);
        }
    }

    public void DropWeapon()
    {
        sphereCollider.enabled = true;
    }
}
