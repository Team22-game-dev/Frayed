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

    private List<GameObject> storedItems;
    private HashSet<GameObject> storedItemsSet;

    private MC_EquippedWeapon mcEquippedWeapon;
    private InputManager inputManager;

    private readonly GameObject hand = null;

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
        inventoryIndex = 0;

        storedItemsSet = new HashSet<GameObject>();

        mcEquippedWeapon = MC_EquippedWeapon.Instance;
        Debug.Assert(mcEquippedWeapon != null);

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        readyToSwitch = true;
    }

    private void Update()
    {
        if (inputManager.toggleInventory)
        {
            Toggle(!_toggled);
            // TODO: Temp logic to demonstrate functional item switching via tab key.
            Next();
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
        yield return StartCoroutine(Equip(storedItems[storedItems.Count - 1]));
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

}
