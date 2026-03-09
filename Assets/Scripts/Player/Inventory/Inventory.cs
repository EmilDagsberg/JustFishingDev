using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public Item[] hotbar = new Item[5];
    public Item[] backpack = new Item[15];

    void Awake()
    {
        instance = this;
    }

    public bool AddItem(Item item)
    {
        // Try hotbar first
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i] == null)
            {
                hotbar[i] = item;
                return true;
            }
        }
        // Then backpack
        for (int i = 0; i < backpack.Length; i++)
        {
            if (backpack[i] == null)
            {
                backpack[i] = item;
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }
}