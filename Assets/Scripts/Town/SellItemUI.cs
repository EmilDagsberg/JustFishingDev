using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI priceText;
    public Button sellButton;

    private FishData fish;
    private ShopUI shopUI;

    public void Setup(FishData newFish, int amount, ShopUI manager)
    {
        fish = newFish;
        shopUI = manager;

        icon.sprite = fish.icon;
        nameText.text = fish.fishName;
        amountText.text = "x" + amount;
        priceText.text = "Sell Price: " + fish.sellValue.ToString();

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(SellOne);
    }

    void SellOne()
    {
        shopUI.SellFish(fish);
    }
}
