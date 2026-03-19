using System.Collections;
using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    [Header("Water Detection")]
    [SerializeField] private LayerMask waterLayer;

    [Header("Floating")]
    [SerializeField] private bool freezeOnWaterHit = true;
    [SerializeField] private float submergeOffset = 0.5f;
    [SerializeField] private float settleDuration = 0.15f;

    private FishingRodController rod;
    private Rigidbody rb;

    public bool IsInWater { get; private set; }
    public FishingWater CurrentWater { get; private set; }

    public void Initialize(FishingRodController fishingRod)
    {
        rod = fishingRod;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckWater(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckWater(other.gameObject);
    }

    private void CheckWater(GameObject other)
    {
        if (IsInWater) return;

        if (((1 << other.layer) & waterLayer) != 0)
        {
            IsInWater = true;
            CurrentWater = other.GetComponentInParent<FishingWater>();

            StartCoroutine(SettleOnWater(other));

            rod?.NotifyBobberHitWater(this);
        }
    }

    private IEnumerator SettleOnWater(GameObject waterObject)
    {
        Collider waterCollider = waterObject.GetComponent<Collider>();
        if (waterCollider == null)
            yield break;

        float waterSurfaceY = waterCollider.bounds.max.y;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, waterSurfaceY - submergeOffset, startPos.z);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
        }

        float timer = 0f;

        while (timer < settleDuration)
        {
            timer += Time.deltaTime;
            float t = timer / settleDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;

        if (freezeOnWaterHit && rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public IEnumerator PlayDipAnimation(float dipDistance, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 dippedPos = startPos + Vector3.down * dipDistance;

        float halfDuration = duration * 0.5f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.position = Vector3.Lerp(startPos, dippedPos, t);
            yield return null;
        }

        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.position = Vector3.Lerp(dippedPos, startPos, t);
            yield return null;
        }

        transform.position = startPos;
    }
}