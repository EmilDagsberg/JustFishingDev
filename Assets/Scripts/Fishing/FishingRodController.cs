using System.Collections;
using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private ItemData fishItem; // item added to inventory when caught

    [Header("Cast Settings")]
    [SerializeField] private float castForce = 15f;
    [SerializeField] private float upwardForce = 2f;

    [Header("Bite Settings")]
    [SerializeField] private float minBiteTime = 5f;
    [SerializeField] private float maxBiteTime = 30f;
    [SerializeField] private float bobberDipDistance = 0.25f;
    [SerializeField] private float bobberDipDuration = 1f;

    private PlayerInventory inventory;
    private FishingBobber currentBobber;
    private Coroutine biteRoutine;

    private bool lineCast;
    private bool fishCaughtOnLine;

    private void Start()
    {
        inventory = GetComponentInParent<PlayerInventory>();

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

        if (fishCaughtOnLine && fishItem != null && inventory != null)
        {
            inventory.Add(fishItem);
            Debug.Log("You caught a fish!");
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

        fishCaughtOnLine = true;
        yield return StartCoroutine(currentBobber.PlayDipAnimation(bobberDipDistance, bobberDipDuration));
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