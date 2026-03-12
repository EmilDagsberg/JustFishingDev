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

    bool isOwned = false;

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
        if (isOwned)
        {
            buyButton.interactable = false;
            priceText.text = "Owned";

            if (canvasGroup != null)
                canvasGroup.alpha = 0.5f;

            return;
        }

        bool canAfford = currentCoins >= item.price;

        buyButton.interactable = canAfford;

        if (canvasGroup != null)
            canvasGroup.alpha = canAfford ? 1f : 0.55f;

        priceText.text = item.price.ToString();
        priceText.color = canAfford ? Color.white : Color.red;
    }

    public void MarkOwned()
    {
        isOwned = true;
        RefreshAffordableState(0);
    }

    void BuyItem()
    {
        shopManager.BuyItem(item, this);
    }
}