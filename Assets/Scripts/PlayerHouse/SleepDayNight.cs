using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Light directionalLight;
    [SerializeField] Image fadePanel;
    [SerializeField] GameObject interactPromptUI;

    [Header("Skyboxes")]
    [SerializeField] Material[] daySkyboxes;
    [SerializeField] Material[] nightSkyboxes;

    [Header("Lighting")]
    [SerializeField] Color dayLightColor = Color.white;
    [SerializeField] Color nightLightColor = new Color(0.2f, 0.3f, 0.5f);
    [SerializeField] float dayLightIntensity = 1.0f;
    [SerializeField] float nightLightIntensity = 0.1f;

    [Header("Raycast")]
    [SerializeField] float rayDistance = 3f;

    GameObject house;
    bool isLookingAtHouse = false;
    bool isDay = true;
    bool isFading = false;
    bool inRange = false;

    void Start()
    {
        house = GameObject.FindWithTag("House");
        if (house == null)
        {
            Debug.LogError("No GameObject with tag 'House' found in the scene.");
        }
    }

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogError("playerCamera is not assigned!");
            return;
        }

        // Draw debug ray
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayDistance, Color.green);

        // Reset bools
        isLookingAtHouse = false;
        inRange = false;

        // Raycast and log what is hit
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("House"))
            {
                isLookingAtHouse = true;
                float distance = Vector3.Distance(playerCamera.transform.position, hit.collider.transform.position);
                inRange = true;
            }
        }

        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(isLookingAtHouse && inRange && !isFading);
        }

        // Toggle day/night if looking at house and pressing E
        if (isLookingAtHouse && inRange && Input.GetKeyDown(KeyCode.E) && !isFading)
        {
            StartCoroutine(SleepSequence());
        }
    }

    void ToggleDayNight()
    {
        isDay = !isDay;

        if (isDay)
        {
            if (daySkyboxes.Length > 0)
                RenderSettings.skybox = daySkyboxes[Random.Range(0, daySkyboxes.Length)];
            if (directionalLight != null)
            {
                RenderSettings.skybox = daySkyboxes[Random.Range(0, daySkyboxes.Length)];
                directionalLight.color = dayLightColor;
                directionalLight.intensity = dayLightIntensity;
                directionalLight.shadows = LightShadows.Soft;
                RenderSettings.ambientIntensity = 1.0f;
            }
        }
        else
        {
            if (nightSkyboxes.Length > 0)
                RenderSettings.skybox = nightSkyboxes[Random.Range(0, nightSkyboxes.Length)];
            if (directionalLight != null)
            {
                RenderSettings.skybox = nightSkyboxes[Random.Range(0, nightSkyboxes.Length)];
                directionalLight.color = nightLightColor;
                directionalLight.intensity = nightLightIntensity;
                directionalLight.shadows = LightShadows.Hard;
                RenderSettings.ambientIntensity = 0.5f;
            }
        }

        DynamicGI.UpdateEnvironment();
    }


    // Function to make it seem like player is falling asleep
    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = fadePanel.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadePanel.color = color;
            yield return null;
        }
        color.a = endAlpha;
        fadePanel.color = color;
    }

    IEnumerator SleepSequence()
    {
        isFading = true;
        yield return StartCoroutine(Fade(0f,1f,1f)); // Fade to black
        ToggleDayNight();
        yield return new WaitForSeconds(0.5f); // Pauses
        yield return StartCoroutine(Fade(1f,0f,1f)); // Fade back in
        isFading = false;
    }
}