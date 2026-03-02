using UnityEditor;
using UnityEngine;

public class WaterInteraction : MonoBehaviour
{

    // TODO:
    // Check what waterObject is closest to the player (DONE) -> void Update()
    // Check if water is in front (Cone shape maybe)
    // Need to register throw from player -> Water surface
    // Disable player movement when minigame is ongoing
    // Start and stop for minigame

    [SerializeField] GameObject[] waterPrefab;
    [SerializeField] GameObject player;
    [SerializeField] float rayDistance = 100f;
    [SerializeField] GameObject fishingIcon;

    [Header("Player controller")]
    [SerializeField] PlayerController playerController; // Reference to controller

    bool isLookingAtWater;
    bool minigameActive = false;

    void Update()
    {
        // NearestWater();
        LookingAtWater();
        MinigameCheck();
    }


    void NearestWater()
    {
        if (player == null)
        {
            Debug.Log("Player not assigned in inspector");
        }

        float minDistance = float.MaxValue;
        GameObject closestWater = null;

        foreach (var water in waterPrefab)
        {
            float dist = Vector3.Distance(water.transform.position, player.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestWater = water;
            }
        }
        Debug.Log("Closest water is: " + closestWater.name + " at distance: " + minDistance);
    }


    void LookingAtWater()
    {
        isLookingAtWater = false;

        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        Debug.DrawRay(origin, direction * rayDistance, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Water"))
            {
                isLookingAtWater = true;
                //Debug.Log("Player is looking at water: " + hit.collider.gameObject.name);
            }
        }

        fishingIcon.SetActive(isLookingAtWater);
    }


    void MinigameCheck()
    {
        if (isLookingAtWater == true && !minigameActive && Input.GetButtonDown("Interact"))
        {
            minigameActive = true;
            playerController.enabled = false; // Stop player movement
            StartMinigame();
        }
    }

    void StartMinigame()
    {
        Debug.Log("Minigame starting...");

    }

    void EndMinigame()
    {
        minigameActive = false;
        playerController.enabled = true; // Start player movement
    }
}
