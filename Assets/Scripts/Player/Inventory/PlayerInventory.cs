using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<PlayerItemData> items = new List<PlayerItemData>();

    public bool Add(PlayerItemData item)
    {
        if (item == null) return false;

        items.Add(item);
        Debug.Log($"Added to inventory: {item.itemName}");
        return true;
    }

    public PlayerItemData Get(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}