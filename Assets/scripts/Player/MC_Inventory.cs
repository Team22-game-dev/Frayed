using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;

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
    private List<GameObject> storedItems;
    private HashSet<GameObject> storedItemsSet;

    private MC_EquippedWeapon mcEquippedWeapon;
    private InputManager inputManager;
    private OptionsMenu optionsMenu;

    private readonly GameObject hand = null;
    private readonly int handInventoryIndex = 0;

    private bool readyToSwitch;

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
        storedItems = new List<GameObject>();
        storedItems.Add(hand);
        inventoryIndex = handInventoryIndex;

        storedItemsSet = new HashSet<GameObject>();

        mcEquippedWeapon = MC_EquippedWeapon.Instance;
        Debug.Assert(mcEquippedWeapon != null);

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        optionsMenu = OptionsMenu.Instance;
        Debug.Assert(inputManager != null);

        readyToSwitch = true;
    }

    private void Update()
    {
        if (inputManager.toggleInventory)
        {
            if (!optionsMenu.toggled)
            {
                Toggle(!_toggled);
                // TODO: Temp logic to demonstrate functional item switching via tab key.
                Next();
            }
            inputManager.toggleInventory = false;
        }
    }

    public void Store(GameObject item, bool unequip = true)
    {
        if (!Contains(item))
        {
            storedItems.Add(item);
            storedItemsSet.Add(item);
        }
        if (unequip)
        {
            item.transform.SetParent(this.transform);
            item.SetActive(false);
        }
    }

    public IEnumerator StoreAndEquip(GameObject item)
    {
        Store(item, false);
        yield return StartCoroutine(Switch(storedItems.Count - 1));
    }

    private IEnumerator Equip(GameObject item)
    {
        Debug.Assert(Contains(item));
        mcEquippedWeapon.UnEquipWeapon();
        yield return StartCoroutine(mcEquippedWeapon.StartEquipWeaponCoroutine(item));
        item.SetActive(true);
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
            readyToSwitch = true;
        }
        else
        {
            Debug.Log($"Was not ready to switch to item {index}. Skipped.");
        }
    }

    public bool Contains(GameObject item)
    {
        return storedItemsSet.Contains(item);
    }

    public void Toggle(bool state)
    {
        _toggled = state;
        if (_toggled)
        {

        }
        else
        {

        }
    }

    public IEnumerator ClearInventory()
    {
        yield return StartCoroutine(Switch(handInventoryIndex));
        for (int i = storedItems.Count-1; i >= 1; --i)
        {
            GameObject item = storedItems[i];
            storedItems.RemoveAt(i);
            storedItemsSet.Remove(item);
            Destroy(item);
        }
    }

}
