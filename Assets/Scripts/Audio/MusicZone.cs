using System.Collections;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private float maxVolume = 0.5f;
    [SerializeField] private float fadeDuration = 2f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(maxVolume));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(0f));
        }
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}