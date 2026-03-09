using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    public Image[] slots;
    public Image[] itemIcons;
    public TMP_Text[] amountTexts;

    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    int selectedSlot = 0;
    float scrollInput;

    void Start()
    {
        SelectSlot(0);
        RefreshUI();
    }

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }

        scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");

        if (scrollInput > 0f)
            SelectSlot((selectedSlot - 1 + slots.Length) % slots.Length);
        else if (scrollInput < 0f)
            SelectSlot((selectedSlot + 1) % slots.Length);
    }

    void SelectSlot(int index)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].color = defaultColor;

        selectedSlot = index;
        slots[selectedSlot].color = selectedColor;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = Inventory.instance.hotbar[i];

            if (slot != null && !slot.IsEmpty())
            {
                itemIcons[i].sprite = slot.fish.icon;
                itemIcons[i].enabled = true;

                // Show stack number
                if (slot.amount > 1)
                    amountTexts[i].text = slot.amount.ToString();
                else
                    amountTexts[i].text = "";
            }
            else
            {
                itemIcons[i].sprite = null;
                itemIcons[i].enabled = false;
                amountTexts[i].text = "";
            }
        }
    }
}