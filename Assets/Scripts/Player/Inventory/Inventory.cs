using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("Runtime Inventory")]
    public Item[] hotbar = new Item[5];
    public Item[] backpack = new Item[15];

    [Header("Starting Inventory")]
    public Item[] startingHotbar = new Item[5];
    public Item[] startingBackpack = new Item[15];

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadStartingInventory();
    }

    void Start()
    {
        HotbarUI hotbarUI = FindObjectOfType<HotbarUI>();
        if (hotbarUI != null)
            hotbarUI.RefreshUI();
    }

    void LoadStartingInventory()
    {
        for (int i = 0; i < hotbar.Length; i++)
        {
            hotbar[i] = startingHotbar[i];
        }

        for (int i = 0; i < backpack.Length; i++)
        {
            backpack[i] = startingBackpack[i];
        }
    }

    public bool AddItem(Item item)
    {
        // Try hotbar first
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i] == null)
            {
                hotbar[i] = item;
                RefreshAllUI();
                return true;
            }
        }

        // Then backpack
        for (int i = 0; i < backpack.Length; i++)
        {
            if (backpack[i] == null)
            {
                backpack[i] = item;
                RefreshAllUI();
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }

    public void RefreshAllUI()
    {
        HotbarUI hotbarUI = FindObjectOfType<HotbarUI>();
        if (hotbarUI != null)
            hotbarUI.RefreshUI();
    }
}