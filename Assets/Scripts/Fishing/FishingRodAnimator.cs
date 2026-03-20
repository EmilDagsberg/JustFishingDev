using UnityEngine;
using System.Collections;

public class FishingRodAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private FishingRodController fishingRodController;

    [Header("Idle Sway")]
    [SerializeField] private float idleSwayAmount = 0.02f;
    [SerializeField] private float idleSwaySpeed = 1.5f;
    [SerializeField] private float idleRotationAmount = 1.5f;

    [Header("Movement Bob")]
    [SerializeField] private float bobAmount = 0.04f;
    [SerializeField] private float bobSpeed = 7f;
    [SerializeField] private float bobRotationAmount = 2f;

    [Header("Cast Animation")]
    [SerializeField] private float castBackAngle = 25f;
    [SerializeField] private float castForwardAngle = 60f;
    [SerializeField] private float castBackDuration = 0.15f;
    [SerializeField] private float castForwardDuration = 0.2f;
    [SerializeField] private float returnDuration = 0.2f;
    [SerializeField] private float bobberReleaseTime = 0.05f;

    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;

    private float timer;
    private bool isCasting;
    private bool isMoving;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    void Update()
    {
        if (isCasting)
            return;

        UpdateMovementState();
        UpdateTimer();
        HandleIdleOrMovement();
        HandleCastInput();
    }

    void UpdateMovementState()
    {
        if (playerMovement == null)
        {
            isMoving = false;
            return;
        }

        isMoving = playerMovement.CurrentMovement.magnitude > 0.1f;
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime * (isMoving ? bobSpeed : idleSwaySpeed);
    }

    void HandleIdleOrMovement()
    {
        if (isMoving)
            ApplyMovementBob();
        else
            ApplyIdleSway();
    }

    void HandleCastInput()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        if (fishingRodController == null)
            return;

        if (fishingRodController.HasLineCast())
        {
            fishingRodController.ReelLine();
            return;
        }

        StartCoroutine(PlayCastAnimation());
    }

    void ApplyIdleSway()
    {
        float swayX = Mathf.Sin(timer) * idleSwayAmount;
        float swayY = Mathf.Cos(timer * 0.8f) * idleSwayAmount;
        float rotZ = Mathf.Sin(timer) * idleRotationAmount;

        Vector3 targetPosition = startLocalPosition + new Vector3(swayX, swayY, 0f);
        Quaternion targetRotation = startLocalRotation * Quaternion.Euler(0f, 0f, rotZ);

        ApplyTransform(targetPosition, targetRotation);
    }

    void ApplyMovementBob()
    {
        float bobX = Mathf.Cos(timer * 0.5f) * bobAmount;
        float bobY = Mathf.Sin(timer) * bobAmount;
        float rotZ = Mathf.Sin(timer) * bobRotationAmount;

        Vector3 targetPosition = startLocalPosition + new Vector3(bobX, bobY, 0f);
        Quaternion targetRotation = startLocalRotation * Quaternion.Euler(0f, 0f, rotZ);

        ApplyTransform(targetPosition, targetRotation);
    }

    void ApplyTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 8f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * 8f);
    }

    IEnumerator PlayCastAnimation()
    {
        isCasting = true;

        Quaternion backRotation = startLocalRotation * Quaternion.Euler(-castBackAngle, 0f, 0f);
        Quaternion forwardRotation = startLocalRotation * Quaternion.Euler(castForwardAngle, 0f, 0f);

        yield return RotateRod(transform.localRotation, backRotation, castBackDuration);
        yield return RotateAndRelease(backRotation, forwardRotation, castForwardDuration);

        yield return RotateRod(forwardRotation, startLocalRotation, returnDuration);

        isCasting = false;
    }

    IEnumerator RotateAndRelease(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;
        bool bobberReleased = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localRotation = Quaternion.Lerp(from, to, t);

            if (!bobberReleased && elapsed >= bobberReleaseTime)
            {
                bobberReleased = true;
                fishingRodController.CastLine();
            }

            yield return null;
        }

        transform.localRotation = to;
    }

    IEnumerator RotateRod(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }

        transform.localRotation = to;
    }
}