using UnityEngine;

public class SleepDayNight : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject house;
    [SerializeField] GameObject player;

    [Header("Skyboxes")]
    [SerializeField] Material[] daySkyboxes;
    [SerializeField] Material[] nightSkyboxes;

    [Header("Lighting")]
    [SerializeField] Light directionalLight;
    [SerializeField] Color dayLightColor = Color.white;
    [SerializeField] Color nightLightColor = new Color(0.2f, 0.3f, 0.5f);
    [SerializeField] float dayLightIntensity = 1.0f;
    [SerializeField] float nightLightIntensity = 0.1f;

    bool isDay = true;
    bool playerInRange = false;

    void changeTime()
    {
        isDay = !isDay;

        if (isDay)
        {
            RenderSettings.skybox = daySkyboxes[Random.Range(0, daySkyboxes.Length)];
            directionalLight.color = dayLightColor;
            directionalLight.intensity = dayLightIntensity;
            directionalLight.shadows = LightShadows.Soft;
            RenderSettings.ambientIntensity = 1.0f;
        }
        else
        {
            RenderSettings.skybox = nightSkyboxes[Random.Range(0, nightSkyboxes.Length)];
            directionalLight.color = nightLightColor;
            directionalLight.intensity = nightLightIntensity;
            directionalLight.shadows = LightShadows.Hard;
            RenderSettings.ambientIntensity = 0.2f;
        }

        DynamicGI.UpdateEnvironment();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
            playerInRange = false;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            changeTime();
    }
}