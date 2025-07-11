using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MC_Inventory : MonoBehaviour
{

    // Singleton Design
    private static MC_Inventory _instance;
    public static MC_Inventory Instance => _instance;

    public bool toggled { get { return _toggled; } private set { _toggled = value; } }

    [SerializeField]
    private bool _toggled = false;

    [SerializeField]
    private int inventoryIndex;

    [SerializeField]
    private int desiredInventoryIndex;

    [SerializeField]
    private List<InventoryItem> storedItems;

    private HashSet<string> storedItemsSet;

    private MC_EquippedWeapon mcEquippedWeapon;
    private InputManager inputManager;

    private InventoryItem hand;
    private readonly int handInventoryIndex = 0;

    [SerializeField]
    private bool readyToSwitch;
    [SerializeField]
    private float timeSinceSwitch;
    [SerializeField]
    private float readyToSwitchTimeout = 3.0f;

    private GameObject canvasGameObject;
    private Image image;
    private TMP_Text text;
    private TMP_Text indexText;

    private void Awake()
    {
        // Singleton pattern with explicit null check
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        hand = this.transform.Find("Hand").GetComponent<WeaponData>();
        Debug.Assert(hand != null);

        storedItems = new List<InventoryItem>();
        storedItemsSet = new HashSet<string>();

        Store(hand);
        inventoryIndex = handInventoryIndex;
        desiredInventoryIndex = handInventoryIndex;

        mcEquippedWeapon = MC_EquippedWeapon.Instance;
        Debug.Assert(mcEquippedWeapon != null);

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        canvasGameObject = this.transform.Find("Canvas").gameObject;
        Debug.Assert(canvasGameObject != null);

        image = this.transform.Find("Canvas/Circle/Image").GetComponent<Image>();
        Debug.Assert(image != null);

        text = this.transform.Find("Canvas/Circle/Text").GetComponent<TMP_Text>();
        Debug.Assert(text != null);

        indexText = this.transform.Find("Canvas/Circle/Index").GetComponent<TMP_Text>();
        Debug.Assert(text != null);

        readyToSwitch = true;
        timeSinceSwitch = 0.0f;

        Toggle(false);
    }

    private void Update()
    {

        Toggle(inputManager.toggleInventory);
        if (_toggled)
        {
            if (inputManager.inventoryNextItem)
            {
                Next();
            }
            else if (inputManager.inventoryPrevItem)
            {
                Prev();
            }
            else if (inputManager.inventoryDropWeapon)
            {
                StartCoroutine(DropWeapon(desiredInventoryIndex));
            }
        }
        else
        {
            if (desiredInventoryIndex != inventoryIndex)
            {
                StartCoroutine(Switch(desiredInventoryIndex));
            }
        }

        // TODO: Temp logic to reset readyToSwitch if error occurs while equipping since finally statements are wonky with coroutines.
        timeSinceSwitch += Time.deltaTime;
        if (timeSinceSwitch > readyToSwitchTimeout && !readyToSwitch)
        {
            readyToSwitch = true;
        }

        // Reset inventory input manager buttons.
        inputManager.inventoryDropWeapon = false;
    }

    public void Store(InventoryItem item, bool unequip = true)
    {
        if (!Contains(item))
        {
            storedItems.Add(item);
            storedItemsSet.Add(item.getInventoryName());
        }
        GameObject itemGameObject = item.gameObject;
        if (unequip)
        {
            item.transform.SetParent(this.transform);
            itemGameObject.SetActive(false);
        }
    }

    public IEnumerator StoreAndEquip(InventoryItem item)
    {
        Store(item, false);
        yield return StartCoroutine(Switch(storedItems.Count - 1));
    }

    public IEnumerator DropWeapon(int index)
    {
        InventoryItem item = storedItems[index];
        if (item == hand)
        {
            yield break;
        }
        if (index == inventoryIndex)
        {
            mcEquippedWeapon.DropWeapon();
            inventoryIndex = handInventoryIndex;
            desiredInventoryIndex = handInventoryIndex;
        }
        else
        {
            int currentIndex = inventoryIndex;
            yield return Switch(index);
            mcEquippedWeapon.DropWeapon();
            inventoryIndex = handInventoryIndex;
            if (currentIndex > index)
            {
                desiredInventoryIndex = currentIndex - 1;
            }
            else
            {
                desiredInventoryIndex = currentIndex;
            }
        }
        // Not the most efficent but it's okay.
        bool deletedFromList = storedItems.Remove(item);
        bool deletedFromSet = storedItemsSet.Remove(item.getInventoryName());
        UpdateInventoryUI();
        if (storedItems.Count == 1)
        {
            mcEquippedWeapon.ResetPlayerAnimation();
        }
        Debug.Assert(deletedFromList);
        Debug.Assert(deletedFromSet);
    }

    private IEnumerator Equip(InventoryItem item)
    {
        Debug.Assert(Contains(item));
        GameObject itemGameObject = item.gameObject;
        yield return StartCoroutine(mcEquippedWeapon.StartEquipWeaponCoroutine(itemGameObject));
        itemGameObject.SetActive(true);
        mcEquippedWeapon.DrawWeapon(false);
    }

    private void Prev()
    {
        if (storedItems.Count <= 1)
        {
            return;
        }
        desiredInventoryIndex = (desiredInventoryIndex + storedItems.Count - 1) % storedItems.Count;
        UpdateInventoryUI();
    }

    private void Next()
    {
        if (storedItems.Count <= 1)
        {
            return;
        }
        desiredInventoryIndex = (desiredInventoryIndex + 1) % storedItems.Count;
        UpdateInventoryUI();
    }

    private IEnumerator Switch(int index)
    {
        if (readyToSwitch)
        {
            readyToSwitch = false;
            InventoryItem currentItem = storedItems[inventoryIndex];
            if (currentItem != hand && mcEquippedWeapon.currentWeaponState == EquippedWeaponBase.WeaponState.Drawn)
            {
                mcEquippedWeapon.SheathAndDrawWeapon();
            }
            mcEquippedWeapon.UnEquipWeapon(currentItem.transform.parent);
            if (storedItems[index] != hand)
            {
                yield return StartCoroutine(Equip(storedItems[index]));
            }
            inventoryIndex = index;
            desiredInventoryIndex = index;
            UpdateInventoryUI();
            timeSinceSwitch = 0.0f;
            readyToSwitch = true;
        }
        else
        {
            Debug.Log($"Was not ready to switch to item {index}. Skipped.");
        }
    }

    private void UpdateInventoryUI()
    {
        InventoryItem item = storedItems[desiredInventoryIndex];
        image.sprite = item.getInventorySprite();
        text.text = item.getInventoryName();
        if (storedItems.Count <= 1)
        {
            indexText.text = "1";
        }
        else
        {
            indexText.text = $"<  {desiredInventoryIndex + 1}  >";
        }
    }

    public bool Contains(InventoryItem item)
    {
        return Contains(item.getInventoryName());
    }

    public bool Contains(string name)
    {
        return storedItemsSet.Contains(name);
    }

    public void Toggle(bool state)
    {
        if (_toggled)
        {
            UpdateInventoryUI();
            canvasGameObject.SetActive(true);
        }
        else
        {
            canvasGameObject.SetActive(false);
        }
        _toggled = state;
    }

    public void ClearInventory()
    {
        for (int i = storedItems.Count - 1; i >= 1; --i)
        {
            InventoryItem item = storedItems[i];
            storedItems.RemoveAt(i);
            storedItemsSet.Remove(item.getInventoryName());
            Destroy(item.gameObject);
        }
        foreach (Transform sheath in mcEquippedWeapon.GetWeaponBoneData["WeaponSheathHip"])
        {
            Destroy(sheath.gameObject);
        }
        foreach (Transform sheath in mcEquippedWeapon.GetWeaponBoneData["WeaponSheathBack"])
        {
            Destroy(sheath.gameObject);
        }
        inventoryIndex = handInventoryIndex;
        desiredInventoryIndex = handInventoryIndex;
        mcEquippedWeapon.ResetPlayerAnimation();
        UpdateInventoryUI();
    }

}
