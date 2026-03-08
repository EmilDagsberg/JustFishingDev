using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private GameObject shopMenuUI;

    [Header("Shop System")]
    [SerializeField] private ItemData[] shopItems;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool shopOpen = false;
    private int coins = 100;
    private bool shopGenerated = false;

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
        {
            CloseShop();
        }
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
        foreach (ItemData item in shopItems)
        {
            GameObject obj = Instantiate(itemPrefab, itemContainer);

            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.Setup(item, this);
        }
    }

    public void BuyItem(ItemData item)
    {
        if (coins >= item.price)
        {
            coins -= item.price;
            UpdateCoins();

            Debug.Log("Bought: " + item.itemName);
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    void UpdateCoins()
    {
        coinText.text = "Coins: " + coins;
    }
}