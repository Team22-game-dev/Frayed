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

    private BoxCollider boxCollider;

    private EquippedWeaponBase newUser;

    private MC_Inventory mcInventory;

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

        mcInventory = MC_Inventory.Instance;
        Debug.Assert(mcInventory != null);

        CreatePickupUI();

        // Set layer to "Weapons"
        gameObject.layer = LayerMask.NameToLayer("Weapons");
    }

    private void CreatePickupUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("PickupCanvas");
        canvasGO.transform.SetParent(transform);
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
                if (user.WillPickupWeapon() && pickupPromptUI.enabled)
                {
                    newUser = user;
                    StartCoroutine(PickupWeapon());
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
        {
            if (!mcInventory.Contains(this.gameObject))
            {
                pickupPromptUI.enabled = true;
            }
        }
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

    private IEnumerator PickupWeapon()
    {
        boxCollider = GetComponent<BoxCollider>();


        if (boxCollider == null || sphereCollider == null || rigidBody == null)
        {
            Debug.LogError("Missing required components on weapon.");
            yield break;
        }

        pickupPromptUI.enabled = false;

        // Disable pickup detection
        sphereCollider.enabled = false;

        // Prepare damage collider
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
        // boxCollider.isTrigger = true;

        // Disable physics for equipped state
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;

        // Don't deactivate — just wait one frame to ensure collider states update
        yield return null;

        // Assign user and handle equip
        if (newUser == null)
        {
            Debug.LogError("Attempted to equip a weapon with a null user.");
            yield break;
        }

        if (newUser.gameObject.CompareTag("Player"))
        {
            if (newUser.hasWeaponEquipped())
            {
                mcInventory.Store(this.gameObject);
            }
            else
            {
                yield return StartCoroutine(mcInventory.StoreAndEquip(this.gameObject));
            }
        }
        else
        {
            yield return newUser.StartEquipWeaponCoroutine(this.gameObject);
            newUser.DrawWeapon(false);
        }

        // Optional: Set layer after successful pickup
        gameObject.layer = LayerMask.NameToLayer("Weapons");
    }


    public void DropWeapon()
    {
        sphereCollider.enabled = true;
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }
}
