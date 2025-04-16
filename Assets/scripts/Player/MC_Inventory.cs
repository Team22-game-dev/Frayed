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
    private List<InventoryItem> storedItems;

    private HashSet<string> storedItemsSet;

    private MC_EquippedWeapon mcEquippedWeapon;
    private InputManager inputManager;
    private OptionsMenu optionsMenu;

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
        storedItems.Add(hand);
        inventoryIndex = handInventoryIndex;

        storedItemsSet = new HashSet<string>();

        mcEquippedWeapon = MC_EquippedWeapon.Instance;
        Debug.Assert(mcEquippedWeapon != null);

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        optionsMenu = OptionsMenu.Instance;
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

        Toggle(true);
    }

    private void Update()
    {
        if (inputManager.toggleInventory)
        {
            if (!optionsMenu.toggled)
            {
                //Toggle(!_toggled);
                // TODO: Temp logic to demonstrate functional item switching via tab key.
                Next();
            }
            inputManager.toggleInventory = false;
        }

        // TODO: Temp logic to reset readyToSwitch if error occurs while equipping since finally statements are wonky with coroutines.
        timeSinceSwitch += Time.deltaTime;
        if (timeSinceSwitch > readyToSwitchTimeout && !readyToSwitch)
        {
            readyToSwitch = true;
        }
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

    private IEnumerator Equip(InventoryItem item)
    {
        Debug.Assert(Contains(item));
        GameObject itemGameObject = item.gameObject;
        mcEquippedWeapon.UnEquipWeapon();
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
        int nextInventoryIndex = (inventoryIndex + storedItems.Count - 1) % storedItems.Count;
        StartCoroutine(Switch(nextInventoryIndex));
    }

    private void Next()
    {
        if (storedItems.Count <= 1)
        {
            return;
        }
        int nextInventoryIndex = (inventoryIndex + 1) % storedItems.Count;
        StartCoroutine(Switch(nextInventoryIndex));
    }

    private IEnumerator Switch(int index)
    {
        if (readyToSwitch)
        {
            readyToSwitch = false;
            if (storedItems[index] == hand)
            {
                mcEquippedWeapon.UnEquipWeapon();
            }
            else
            {
                yield return StartCoroutine(Equip(storedItems[index]));
            }
            inventoryIndex = index;
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
        InventoryItem item = storedItems[inventoryIndex];
        image.sprite = item.getInventorySprite();
        text.text = item.getInventoryName();
        if (storedItems.Count <= 1)
        {
            indexText.text = "1";
        }
        else
        {
            indexText.text = $"<  {inventoryIndex + 1}  >";
        }
    }

    public bool Contains(InventoryItem item)
    {
        return storedItemsSet.Contains(item.getInventoryName());
    }

    public void Toggle(bool state)
    {
        _toggled = state;
        if (_toggled)
        {
            UpdateInventoryUI();
            canvasGameObject.SetActive(true);
        }
        else
        {
            canvasGameObject.SetActive(false);
        }
    }

    public IEnumerator ClearInventory()
    {
        yield return StartCoroutine(Switch(handInventoryIndex));
        for (int i = storedItems.Count - 1; i >= 1; --i)
        {
            InventoryItem item = storedItems[i];
            storedItems.RemoveAt(i);
            storedItemsSet.Remove(item.getInventoryName());
            Destroy(item);
        }
    }

}
