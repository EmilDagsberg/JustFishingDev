using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public CanvasGroup canvasGroup;

    ItemData item;
    ShopUI shopManager;

    public void Setup(ItemData newItem, ShopUI manager)
    {
        item = newItem;
        shopManager = manager;

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyItem);
    }

    public void RefreshAffordableState(int currentCoins)
    {
        bool canAfford = currentCoins >= item.price;

        buyButton.interactable = canAfford;

        if (canvasGroup != null)
            canvasGroup.alpha = canAfford ? 1f : 0.55f;

        priceText.color = canAfford ? Color.white : Color.red;
    }

    void BuyItem()
    {
        shopManager.BuyItem(item);
    }
}