using UnityEngine;

public class ItemEquipper : MonoBehaviour
{
    public PlayerInventory inventory;
    public Transform itemHolder;

    private GameObject equippedInstance;

    private void Awake()
    {
        if (inventory == null) inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipIndex(2);
    }

    public void EquipIndex(int index)
    {
        if (inventory == null || itemHolder == null) return;

        ItemData item = inventory.Get(index);
        if (item == null || item.equippedPrefab == null)
        {
            Debug.Log("No item in that slot.");
            return;
        }

        if (equippedInstance != null) Destroy(equippedInstance);

        equippedInstance = Instantiate(item.equippedPrefab, itemHolder);
        equippedInstance.transform.localPosition = Vector3.zero;
        equippedInstance.transform.localRotation = Quaternion.identity;
        equippedInstance.transform.localScale = Vector3.one;

        Debug.Log("Equipped: " + item.itemName);
    }

    // auto-equip newest pickup
    public void EquipLast()
    {
        EquipIndex(inventory.items.Count - 1);
    }
}