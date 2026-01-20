using UnityEngine;
using System;

public struct ClipVolumePitch 
{
    public AudioClip clip;
    public float volume;
    public float pitch;
}

[Serializable]
public class VariedAudioClip
{
    [SerializeField] public AudioClip[] clips;
    [SerializeField] public float volume = 0.5f;
    [SerializeField] public float volumeVariance = 0.1f;
    [SerializeField] public float pitch = 1f;
    [SerializeField] public float pitchVariance = 0.1f;

    public ClipVolumePitch GetRandomizedClip()
    {
        AudioClip clip = GetRandomClip();
        float randomizedVolume = GetRandomVolume();
        float randomizedPitch = GetRandomPitch();

        return new ClipVolumePitch
        {
            clip = clip,
            volume = randomizedVolume,
            pitch = randomizedPitch
        };
    }

    public AudioClip GetRandomClip()
    {
        if (clips.Length == 0)
            throw new Exception("No audio clips assigned to VariedAudioClip");

        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    public float GetRandomVolume()
    {
        return Mathf.Clamp01(volume + UnityEngine.Random.Range(-volumeVariance, volumeVariance));
    }

    public float GetRandomPitch()
    {
        return pitch + UnityEngine.Random.Range(-pitchVariance, pitchVariance);
    }
}