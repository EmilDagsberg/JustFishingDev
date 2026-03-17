using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("Runtime Inventory")]
    public InventorySlot[] hotbar = new InventorySlot[5];
    public InventorySlot[] backpack = new InventorySlot[15];

    [Header("Starting Inventory")]
    public InventorySlot[] startingHotbar = new InventorySlot[5];
    public InventorySlot[] startingBackpack = new InventorySlot[15];

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if  (instance != this)
        {
            Debug.LogWarning("More than one instance of Inventory found on: " + gameObject.name);
            Destroy(this);
            return;
        }

        InitializeSlots(hotbar);
        InitializeSlots(backpack);
        InitializeSlots(startingHotbar);
        InitializeSlots(startingBackpack);

        LoadStartingInventory();
    }

    void Start()
    {
        RefreshAllUI();
    }

    void InitializeSlots(InventorySlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                slots[i] = new InventorySlot();
        }
    }

    void LoadStartingInventory()
    {
        CopySlots(startingHotbar, hotbar);
        CopySlots(startingBackpack, backpack);
    }

    void CopySlots(InventorySlot[] source, InventorySlot[] target)
    {
        for (int i = 0; i < target.Length; i++)
        {
            if (source[i] != null && source[i].fish != null && source[i].amount > 0)
            {
                target[i].fish = source[i].fish;
                target[i].amount = source[i].amount;
            }
            else
            {
                target[i].Clear();
            }
        }
    }

    public bool AddFish(FishData fish, int amount = 1)
    {
        if (fish == null || amount <= 0)
            return false;

        int remaining = amount;

        remaining = AddToExistingStacks(hotbar, fish, remaining);
        remaining = AddToExistingStacks(backpack, fish, remaining);

        remaining = AddToEmptySlots(hotbar, fish, remaining);
        remaining = AddToEmptySlots(backpack, fish, remaining);

        RefreshAllUI();

        if (remaining > 0)
        {
            Debug.Log("Inventory full! Could not add all fish.");
            return false;
        }

        return true;
    }

    int AddToExistingStacks(InventorySlot[] slots, FishData fish, int amountToAdd)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot.fish == fish && slot.amount < fish.maxStack)
            {
                int spaceLeft = fish.maxStack - slot.amount;
                int addAmount = Mathf.Min(spaceLeft, amountToAdd);

                slot.amount += addAmount;
                amountToAdd -= addAmount;

                if (amountToAdd <= 0)
                    return 0;
            }
        }

        return amountToAdd;
    }

    int AddToEmptySlots(InventorySlot[] slots, FishData fish, int amountToAdd)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot.IsEmpty())
            {
                int addAmount = Mathf.Min(fish.maxStack, amountToAdd);

                slot.fish = fish;
                slot.amount = addAmount;
                amountToAdd -= addAmount;

                if (amountToAdd <= 0)
                    return 0;
            }
        }

        return amountToAdd;
    }

    public void RefreshAllUI()
    {
        HotbarUI hotbarUI = FindObjectOfType<HotbarUI>();
        if (hotbarUI != null)
            hotbarUI.RefreshUI();
    }

    public bool RemoveFish(FishData fish, int amount = 1)
    {
        if (fish == null || amount <= 0)
            return false;

        int remaining = amount;

        remaining = RemoveFromSlots(hotbar, fish, remaining);
        remaining = RemoveFromSlots(backpack, fish, remaining);

        RefreshAllUI();
        return remaining <= 0;
    }

int RemoveFromSlots(InventorySlot[] slots, FishData fish, int amountToRemove)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot != null && slot.fish == fish && slot.amount > 0)
            {
                int removeAmount = Mathf.Min(slot.amount, amountToRemove);
                slot.amount -= removeAmount;
                amountToRemove -= removeAmount;

                if (slot.amount <= 0)
                    slot.Clear();

                if (amountToRemove <= 0)
                    return 0;
            }
        }

        return amountToRemove;
    }
}