using UnityEngine;

class AudioPlayer : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(AudioClip clip, float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }    
}