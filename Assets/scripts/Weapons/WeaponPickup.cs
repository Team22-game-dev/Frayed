using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Describes pickup behavior
 * Weapons can be picked up by the player, enemies, and other NPCs
 */
public class WeaponPickup : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Rigidbody rigidBody;

    [SerializeField]
    private WeaponData weaponData;

    [SerializeField]
    private float pickupRadius = 0.5f; 

    [SerializeField]
    private List<EquippedWeaponBase> userWeaponController;

    private EquippedWeaponBase newUser;

    // UI Elements
    [SerializeField]
    private Sprite pickupPrompt;  // Assign this in the Inspector
    private Image pickupPromptUI;
    private Canvas pickupCanvas;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Add sphere collider for pickup detection
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = pickupRadius;
        sphereCollider.enabled = true;

        userWeaponController = new List<EquippedWeaponBase>();

        CreatePickupUI();

        // Set layer to "Weapons"
        gameObject.layer = LayerMask.NameToLayer("Weapons");
    }

private void CreatePickupUI()
{
    // Create Canvas
    GameObject canvasGO = new GameObject("PickupCanvas");
    pickupCanvas = canvasGO.AddComponent<Canvas>();
    pickupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    canvasGO.AddComponent<CanvasScaler>();
    canvasGO.AddComponent<GraphicRaycaster>();

    // Create Image
    GameObject imageGO = new GameObject("PickupPrompt");
    imageGO.transform.SetParent(canvasGO.transform);
    pickupPromptUI = imageGO.AddComponent<Image>();
    pickupPromptUI.sprite = pickupPrompt;
    pickupPromptUI.enabled = false;  // Initially hidden

    // Adjust size while preserving aspect ratio
    RectTransform rt = pickupPromptUI.GetComponent<RectTransform>();

    if (pickupPrompt != null)
    {
        float originalWidth = pickupPrompt.rect.width;
        float originalHeight = pickupPrompt.rect.height;
        float aspectRatio = originalWidth / originalHeight;

        float targetHeight = 50; // Set base height
        float targetWidth = targetHeight * aspectRatio; // Maintain aspect ratio

        rt.sizeDelta = new Vector2(targetWidth, targetHeight);
    }
    else
    {
        rt.sizeDelta = new Vector2(70, 70);  // Fallback default size
    }
}


    private void Update()
    {
        if (userWeaponController.Count > 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 playerWorldPosition = player.transform.position;
                
                // Convert player's world position to screen position
                Vector3 screenPos = Camera.main.WorldToScreenPoint(playerWorldPosition + Vector3.up * .85f);
                
                screenPos.x += 200f;
                // Position UI prompt over player
                pickupPromptUI.rectTransform.position = screenPos;
            }

            foreach (EquippedWeaponBase user in userWeaponController)
            {
                if (user.WillPickupWeapon())
                {
                    newUser = user;
                    PickupWeapon();
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("Enemy") || other.CompareTag("NPC_WeaponUser")))
            return;

        EquippedWeaponBase userComponent = other.GetComponent<EquippedWeaponBase>();
        if (userWeaponController.Contains(userComponent))
            return;

        userWeaponController.Add(userComponent);

        if (other.CompareTag("Player"))
            pickupPromptUI.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        EquippedWeaponBase userComponent = other.GetComponent<EquippedWeaponBase>();
        userWeaponController.Remove(userComponent);

        if (userWeaponController.Count == 0)
        {
            pickupPromptUI.enabled = false;
        }
    }

    private void PickupWeapon()
    {
        pickupPromptUI.enabled = false;
        sphereCollider.enabled = false;
        
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;

        if (newUser == null)
        {
            Debug.LogError("Attempted to equip a weapon with a null user.");
            return;
        }

        if (newUser.hasWeaponEquipped())
        {
            // TODO: Implement logic to send weapon to inventory
        }
        else
        {
            newUser.StartEquipWeaponCoroutine(gameObject);
            newUser.DrawWeapon(false);
        }
    }

    public void DropWeapon()
    {
        sphereCollider.enabled = true;
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }
}
