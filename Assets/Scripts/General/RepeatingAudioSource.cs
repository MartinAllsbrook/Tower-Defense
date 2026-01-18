using UnityEngine;

public class RepeatingAudioSource : MonoBehaviour
{
    [SerializeField] float pitch = 1f;
    [SerializeField] float pitchRange = 0.1f;
    [SerializeField] float volume = 0.5f;
    [SerializeField] float volumeRange = 0.1f;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        audioSource.pitch = pitch + Random.Range(-pitchRange, pitchRange);
        audioSource.volume = volume + Random.Range(-volumeRange, volumeRange);
    }
}
