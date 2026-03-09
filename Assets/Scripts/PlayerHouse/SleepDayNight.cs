using UnityEngine;

public class SleepDayNight : MonoBehaviour
{

    // A house that can be interacted with
    // Screen fades in and out of black
    // Skybox change from day skybox to night skybox
    // Directional light changes to darker colors
    // Boolean state changes from day -> night


    [Header("Objects")]
    [SerializeField] GameObject house;
    [SerializeField] GameObject player;

    [Header("Materials")]
    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;
    [SerializeField] Light directionalLight;
    [SerializeField] Color dayLightColor = Color.white;
    [SerializeField] Color nightLightColor = new Color(0.2f, 0.3f, 0.5f); // bluish
    [SerializeField] float dayLightIntensity = 1.0f;
    [SerializeField] float nightLightIntensity = 0.1f;

    [Header("Vegetation shader")]
    [SerializeField] Material vegetationDay;
    [SerializeField] Material vegetationNight;

    bool isDay = true;
    bool playerInRange = false;


    void changeTime()
    {
        isDay = !isDay;
        Material targetMat = isDay ? vegetationDay : vegetationNight;

        foreach (GameObject veg in GameObject.FindGameObjectsWithTag("Vegetation"))
        {
            veg.GetComponentInChildren<Renderer>().material = targetMat;
        }

        if (isDay)
        {
            RenderSettings.skybox = daySkybox;
            directionalLight.color = dayLightColor;
            directionalLight.intensity = dayLightIntensity;
            directionalLight.shadows = LightShadows.Soft;
            RenderSettings.ambientIntensity = 1.0f;


        }
        else
        {
            RenderSettings.skybox = nightSkybox;
            directionalLight.color = nightLightColor;
            directionalLight.intensity = nightLightIntensity;
            directionalLight.shadows = LightShadows.None;
            RenderSettings.ambientIntensity = 0.2f;


        }

        DynamicGI.UpdateEnvironment();
    }


    // Check if player is in range

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            changeTime();
        }
    }

    // Courtesy of GPT, changes shader doing cycle
}
