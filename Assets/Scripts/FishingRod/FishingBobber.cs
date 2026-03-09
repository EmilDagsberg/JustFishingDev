using System.Collections;
using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    [Header("Water Detection")]
    [SerializeField] private LayerMask waterLayer;

    [Header("Floating")]
    [SerializeField] private bool freezeOnWaterHit = true;

    private FishingRodController rod;
    private Rigidbody rb;

    public bool IsInWater { get; private set; }

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

            if (freezeOnWaterHit && rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            rod?.NotifyBobberHitWater(this);
        }
    }

    public IEnumerator PlayDipAnimation(float dipDistance, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 dippedPos = startPos + Vector3.down * dipDistance;

        float halfDuration = duration * 0.5f;
        float timer = 0f;

        // move down
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.position = Vector3.Lerp(startPos, dippedPos, t);
            yield return null;
        }

        // move back up
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