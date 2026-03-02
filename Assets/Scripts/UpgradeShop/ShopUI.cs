using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private GameObject shopMenuUI;

    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool shopOpen = false;

    void Start()
    {
        interactPromptUI.SetActive(false);
        shopMenuUI.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        bool inRange = distance <= interactionDistance;

        // Show prompt only if close and shop not open
        interactPromptUI.SetActive(inRange && !shopOpen);

        if (inRange && Input.GetKeyDown(interactKey))
        {
            if (!shopOpen)
                OpenShop();
            else
                CloseShop();
        }

        // Auto close if player walks away
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

        Time.timeScale = 0f; // Optional pause
    }

    void CloseShop()
    {
        shopOpen = false;
        shopMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }
}