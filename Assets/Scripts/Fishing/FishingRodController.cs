using System.Collections;
using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform playerCatchPoint;
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Cast Settings")]
    [SerializeField] private float castForce = 15f;
    [SerializeField] private float upwardForce = 2f;

    [Header("Bite Settings")]
    [SerializeField] private float minBiteTime = 5f;
    [SerializeField] private float maxBiteTime = 30f;
    [SerializeField] private float bobberDipDistance = 0.25f;
    [SerializeField] private float bobberDipDuration = 1f;

    [Header("Reel Visual")]
    [SerializeField] private float fishFlyDuration = 0.6f;
    [SerializeField] private float fishArcHeight = 1.5f;
    [SerializeField] private float fishSpinSpeed = 360f;

    private FishingBobber currentBobber;
    private Coroutine biteRoutine;

    private bool lineCast;
    private bool fishCaughtOnLine;
    private FishData hookedFish;

    private Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!lineCast)
                CastLine();
            else
                ReelLine();
        }

        UpdateFishingLine();
    }

    private void CastLine()
    {
        if (bobberPrefab == null || castPoint == null)
        {
            Debug.LogWarning("FishingRodController: Missing bobberPrefab or castPoint.");
            return;
        }

        GameObject bobberObj = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        currentBobber = bobberObj.GetComponent<FishingBobber>();

        if (currentBobber == null)
        {
            Debug.LogError("Bobber prefab needs a FishingBobber component.");
            Destroy(bobberObj);
            return;
        }

        currentBobber.Initialize(this);

        Rigidbody rb = bobberObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwDirection = castPoint.forward * castForce + castPoint.up * upwardForce;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(throwDirection, ForceMode.VelocityChange);
        }

        lineCast = true;
        fishCaughtOnLine = false;
        hookedFish = null;

        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    private void ReelLine()
    {
        if (biteRoutine != null)
        {
            StopCoroutine(biteRoutine);
            biteRoutine = null;
        }

        if (fishCaughtOnLine && hookedFish != null)
        {
            StartCoroutine(AnimateFishToPlayerAndAdd(hookedFish));
        }

        if (currentBobber != null)
            Destroy(currentBobber.gameObject);

        currentBobber = null;
        lineCast = false;
        fishCaughtOnLine = false;

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    public void NotifyBobberHitWater(FishingBobber bobber)
    {
        if (bobber != currentBobber) return;

        if (biteRoutine != null)
            StopCoroutine(biteRoutine);

        biteRoutine = StartCoroutine(WaitForFishBite());
    }

    private IEnumerator WaitForFishBite()
    {
        float waitTime = Random.Range(minBiteTime, maxBiteTime);
        yield return new WaitForSeconds(waitTime);

        if (!lineCast || currentBobber == null || !currentBobber.IsInWater)
            yield break;

        FishingWater water = currentBobber.CurrentWater;
        if (water == null)
        {
            Debug.LogWarning("Bobber is in water, but no FishingWater component was found.");
            yield break;
        }

        FishData[] availableFish = water.AvailableFish;
        if (availableFish == null || availableFish.Length == 0)
        {
            Debug.LogWarning("This water has no fish assigned.");
            yield break;
        }

        hookedFish = availableFish[Random.Range(0, availableFish.Length)];
        fishCaughtOnLine = true;

        yield return StartCoroutine(currentBobber.PlayDipAnimation(bobberDipDistance, bobberDipDuration));
    }

    private IEnumerator AnimateFishToPlayerAndAdd(FishData fish)
    {
        if (fish == null)
            yield break;

        if (fish.worldPrefab == null)
        {
            Debug.LogWarning($"Fish '{fish.fishName}' has no worldPrefab assigned.");
            yield break;
        }

        Vector3 startPos = castPoint.position;
        if (currentBobber != null)
            startPos = currentBobber.transform.position;

        Vector3 endPos = playerCatchPoint != null ? playerCatchPoint.position : transform.position;

        GameObject fishVisual = Instantiate(fish.worldPrefab, startPos, Quaternion.identity);

        Rigidbody fishRb = fishVisual.GetComponent<Rigidbody>();
        if (fishRb != null)
        {
            fishRb.linearVelocity = Vector3.zero;
            fishRb.angularVelocity = Vector3.zero;
            fishRb.isKinematic = true;
            fishRb.useGravity = false;
        }

        Collider[] colliders = fishVisual.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        float timer = 0f;

        while (timer < fishFlyDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fishFlyDuration;

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * fishArcHeight;

            fishVisual.transform.position = pos;
            fishVisual.transform.Rotate(Vector3.up, fishSpinSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        Destroy(fishVisual);

        if (inventory != null)
        {
            bool added = inventory.AddFish(fish, 1);

            if (added)
                Debug.Log("Caught: " + fish.fishName);
            else
                Debug.Log("Inventory full! Could not add fish.");
        }

        hookedFish = null;
    }

    private void UpdateFishingLine()
    {
        if (lineRenderer == null) return;

        if (!lineCast || currentBobber == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, castPoint.position);
        lineRenderer.SetPosition(1, currentBobber.transform.position);
    }
}