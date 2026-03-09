using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    ItemData item;
    ShopUI shopManager;

    public void Setup(ItemData newItem, ShopUI manager)
    {
        item = newItem;
        shopManager = manager;

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();

        buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem()
    {
        shopManager.BuyItem(item);
    }
}