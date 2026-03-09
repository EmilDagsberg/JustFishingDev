using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public Image[] slots;           // hotbar slots
    public Image[] itemIcons;       // icon for slots
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    int selectedSlot = 0;
    float scrollInput;

    void Update()
    {
        // Number keys 1-5
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }

        // Scroll wheel
        scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scrollInput > 0f)
            SelectSlot((selectedSlot - 1 + 5) % 5);
        else if (scrollInput < 0f)
            SelectSlot((selectedSlot + 1) % 5);
    }

    void SelectSlot(int index)
    {
        // Reset all slots
        for (int i = 0; i < slots.Length; i++)
            slots[i].color = defaultColor;

        selectedSlot = index;
        slots[selectedSlot].color = selectedColor;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Item item = Inventory.instance.hotbar[i];
            if (item != null)
            {
                itemIcons[i].sprite = item.icon;
                itemIcons[i].enabled = true;
            }
            else
            {
                itemIcons[i].sprite = null;
                itemIcons[i].enabled = false;
            }
        }
    }
}