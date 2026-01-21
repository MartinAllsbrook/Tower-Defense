using System.Collections;
using UnityEngine;

class AudioPlayer : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public float PlayClip(VariedAudioClip variedClip)
    {
        ClipVolumePitch clip = variedClip.GetRandomizedClip();
        return PlayClip(clip);
    }

    public float PlayClip(ClipVolumePitch clip)
    {
        PlayClip(clip.clip, clip.volume, clip.pitch);
        return clip.clip.length / clip.pitch;
    }

    public void PlayClip(AudioClip clip, float volume, float pitch)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }

    public void PlayRepeating(VariedAudioClip variedClip)
    {
        StartCoroutine(PlayRepeatingCoroutine(variedClip));
    }

    public void StopRepeating()
    {
        StopAllCoroutines();
    }

    IEnumerator PlayRepeatingCoroutine(VariedAudioClip clip)
    {
        while (true)
        {
            float duration = PlayClip(clip.GetRandomizedClip());
            yield return new WaitForSeconds(duration);
        }
    }
}