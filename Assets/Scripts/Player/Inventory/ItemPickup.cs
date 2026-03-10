using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PlayerItemData item;

    private PlayerInventory inv;
    private ItemEquipper equipper;
    private bool inRange;

    private void OnTriggerEnter(Collider other)
    {
        inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        equipper = other.GetComponentInParent<ItemEquipper>();
        inRange = true;

        Debug.Log($"In range to pick up: {item?.itemName}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerInventory>() == null) return;

        inRange = false;
        inv = null;
        equipper = null;
    }

    private void Update()
    {
        if (!inRange || inv == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inv.Add(item))
            {
                equipper?.EquipLast();
                Destroy(gameObject);
            }
        }
    }
}