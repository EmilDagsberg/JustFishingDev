using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public bool Add(ItemData item)
    {
        if (item == null) return false;
        items.Add(item);
        Debug.Log($"Added to inventory: {item.itemName}");
        return true;
    }

    public ItemData Get(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}