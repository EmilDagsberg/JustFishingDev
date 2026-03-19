using UnityEngine;

public class FishingRodSFX : MonoBehaviour
{
    [Header("Audio source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound effects")]
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioClip reelSound;
    [SerializeField] private AudioClip caughtSound;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float volume = 1f;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnBobberThrow()
    {
        PlayClip(throwSound);
    }

    public void OnBobberSplash()
    {
        PlayClip(splashSound);
    }

    public void OnReelLine()
    {
        PlayClip(reelSound);
    }

    public void OnCaughtSound()
    {
        PlayClip(caughtSound);
    }
    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, volume);
    }
}
