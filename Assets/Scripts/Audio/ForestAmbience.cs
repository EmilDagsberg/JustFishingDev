using System.Collections;
using UnityEngine;

public class ForestAmbience : MonoBehaviour
{
    [SerializeField] private AudioClip[] windClips;
    [SerializeField] private float maxVolume = 0.5f;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Randomization")]
    [SerializeField] private float minTimeBetweenSounds = 0.5f;
    [SerializeField] private float maxTimeBetweenSounds = 3f;
    [SerializeField] private float minVolumeMult = 0.7f;
    [SerializeField] private float maxVolumeMult = 1f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private bool playerInZone = false;
    private float currentVolume = 0f;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = 0f;
        audioSource.spatialBlend = 0f;
        StartCoroutine(PlayRandomAmbience());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(maxVolume));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(0f));
        }
    }

    private IEnumerator PlayRandomAmbience()
    {
        while (true)
        {
            if (windClips.Length > 0)
            {
                // Pick a random clip
                AudioClip clip = windClips[Random.Range(0, windClips.Length)];

                // Randomize pitch and volume
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                float volumeMult = Random.Range(minVolumeMult, maxVolumeMult);
                audioSource.volume = currentVolume * volumeMult;

                audioSource.clip = clip;
                audioSource.Play();

                // Wait for clip to finish
                yield return new WaitForSeconds(clip.length / audioSource.pitch);
            }

            // Random gap between sounds
            float gap = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
            yield return new WaitForSeconds(gap);
        }
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = currentVolume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            currentVolume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            audioSource.volume = currentVolume;
            yield return null;
        }

        currentVolume = targetVolume;
        audioSource.volume = currentVolume;
    }
}