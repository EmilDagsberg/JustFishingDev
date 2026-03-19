using System.Collections;
using UnityEngine;
public class MusicZone : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] public int priority = 0;

    [Header("Settings")][Range(0f, 1f)] public float chosenVolume = 1f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private void Start()
    {
        Debug.Log("MusicZone Start called on: " + gameObject.name);
        Debug.Log("MusicManager.Instance is: " + (MusicManager.Instance != null ? "FOUND" : "NULL"));

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();

        Collider col = GetComponent<Collider>();
        GameObject player = GameObject.FindWithTag("Player");
        if (col != null && player != null && col.bounds.Contains(player.transform.position))
            MusicManager.Instance.RequestPlay(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            MusicManager.Instance.RequestPlay(this);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            MusicManager.Instance.RequestStop(this);
    }

    public void FadeTo(float targetVolume)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeVolume(targetVolume));
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