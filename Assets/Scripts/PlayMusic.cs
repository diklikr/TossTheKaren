using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;

    private void Start()
    {
        if (musicClip == null)
        {
            Debug.LogWarning("Assign a music clip in the Inspector.", this);
            return;
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}