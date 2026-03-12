using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private GameObject shopMenuUI;

    [Header("Pages")]
    [SerializeField] private GameObject buyPage;
    [SerializeField] private GameObject sellPage;

    [Header("Buy System")]
    [SerializeField] private ItemData[] shopItems;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemPrefab;

    [Header("Sell System")]
    [SerializeField] private Transform sellItemContainer;
    [SerializeField] private GameObject sellItemPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Tab Buttons")]
    [SerializeField] private UnityEngine.UI.Button buyTabButton;
    [SerializeField] private UnityEngine.UI.Button sellTabButton;

    [Header("Tab Colors")]
    [SerializeField] private Color activeTabColor = new Color(0.8f, 0.9f, 1f);
    [SerializeField] private Color inactiveTabColor = Color.white;

    private bool shopOpen = false;
    private int coins = 0;
    private bool shopGenerated = false;
    private List<ShopItemUI> buyItemUIs = new List<ShopItemUI>();
    private HashSet<ItemData> purchasedItems = new HashSet<ItemData>();

    void Start()
    {
        interactPromptUI.SetActive(false);
        shopMenuUI.SetActive(false);
        UpdateCoins();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool inRange = distance <= interactionDistance;

        interactPromptUI.SetActive(inRange && !shopOpen);

        if (inRange && Input.GetKeyDown(interactKey))
        {
            if (!shopOpen)
                OpenShop();
            else
                CloseShop();
        }

        if (!inRange && shopOpen)
            CloseShop();
    }

    void OpenShop()
    {
        shopOpen = true;
        shopMenuUI.SetActive(true);
        interactPromptUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        if (!shopGenerated)
        {
            GenerateShop();
            shopGenerated = true;
        }

        ShowBuyPage();
        RefreshBuyAffordability();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    void GenerateShop()
    {
        buyItemUIs.Clear();

        foreach (ItemData item in shopItems)
        {
            GameObject obj = Instantiate(itemPrefab, itemContainer);

            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.Setup(item, this);
            buyItemUIs.Add(ui);
        }

        RefreshBuyAffordability();
    }

    void RefreshBuyAffordability()
    {
        for (int i = 0; i < buyItemUIs.Count; i++)
        {
            if (buyItemUIs[i] != null)
                buyItemUIs[i].RefreshAffordableState(coins);
        }
    }

    public void ShowBuyPage()
    {
        buyPage.SetActive(true);
        sellPage.SetActive(false);

        RefreshBuyAffordability();
        SetActiveTab(buyTabButton);
    }

    public void ShowSellPage()
    {
        buyPage.SetActive(false);
        sellPage.SetActive(true);

        RefreshSellShop();

        SetActiveTab(sellTabButton);
    }

    public void RefreshSellShop()
    {
        foreach (Transform child in sellItemContainer)
            Destroy(child.gameObject);

        Dictionary<FishData, int> fishTotals = new Dictionary<FishData, int>();

        AddFishToDictionary(Inventory.instance.hotbar, fishTotals);
        AddFishToDictionary(Inventory.instance.backpack, fishTotals);

        foreach (var pair in fishTotals)
        {
            GameObject obj = Instantiate(sellItemPrefab, sellItemContainer);
            SellItemUI ui = obj.GetComponent<SellItemUI>();
            ui.Setup(pair.Key, pair.Value, this);
        }
    }

    void AddFishToDictionary(InventorySlot[] slots, Dictionary<FishData, int> fishTotals)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot != null && !slot.IsEmpty())
            {
                if (fishTotals.ContainsKey(slot.fish))
                    fishTotals[slot.fish] += slot.amount;
                else
                    fishTotals.Add(slot.fish, slot.amount);
            }
        }
    }

    public void BuyItem(ItemData item, ShopItemUI itemUI)
    {
        if (coins >= item.price)
        {
            coins -= item.price;
            UpdateCoins();

            Debug.Log("Bought: " + item.itemName);

            if (item.prefabToSpawn != null)
            {
                Instantiate(
                    item.prefabToSpawn,
                    item.spawnPosition,
                    Quaternion.Euler(item.spawnRotation)
                );
            }

            // Mark item as owned instead of deleting it
            if (item.canOnlyBuyOnce)
            {
                itemUI.MarkOwned();
            }

            RefreshBuyAffordability();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    public void SellFish(FishData fish)
    {
        if (fish == null) return;

        bool removed = Inventory.instance.RemoveFish(fish, 1);

        if (removed)
        {
            coins += fish.sellValue;
            UpdateCoins();
            RefreshSellShop();
            RefreshBuyAffordability();

            Debug.Log("Sold: " + fish.fishName);
        }
    }

    void UpdateCoins()
    {
        coinText.text = "Coins: " + coins;
    }

    void SetActiveTab(Button activeButton)
    {
        buyTabButton.image.color = inactiveTabColor;
        sellTabButton.image.color = inactiveTabColor;

        buyTabButton.interactable = true;
        sellTabButton.interactable = true;

        activeButton.image.color = activeTabColor;
        activeButton.interactable = false;
    }
}