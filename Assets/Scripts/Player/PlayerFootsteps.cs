using IdyllicFantasyNature;
using System.Collections;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public AudioClip footStepSFX;

    [Header("Randomization")]
    [SerializeField] private float minVolume = 0.8f;
    [SerializeField] private float maxVolume = 1f;

    private void Start()
    {
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        StartCoroutine(PlayFootsteps());
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (playerController != null && playerController.movement.magnitude > 0.1f)
            {
                if (AudioManager.instance == null)
                    AudioManager.instance = FindObjectOfType<AudioManager>();

                if (AudioManager.instance != null)
                {
                    float randomVolume = Random.Range(minVolume, maxVolume);
                    AudioManager.instance.PlaySFX(footStepSFX, randomVolume);
                }
            }

            yield return new WaitForSeconds(0.65f);
        }
    }
}