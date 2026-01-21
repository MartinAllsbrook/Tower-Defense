using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    [SerializeField] int numAudioSources = 128;
    [SerializeField] AudioPlayer audioPlayerPrefab;
    
    ObjectPool<AudioPlayer> audioPlayerPool;

    // TODO: Would it be possible to make this whole class static?
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        audioPlayerPool = new ObjectPool<AudioPlayer>(audioPlayerPrefab, numAudioSources, 0, transform);
    }

    static public void PlayAudioAt(AudioClip clip, Vector3 position, float volume = 0.5f, float pitch = 1f)
    {
        if (instance != null)
        {
            instance.PlayAudioAtPosition(clip, position, volume, pitch);
        }
    }

    static public void PlayAudioAt(VariedAudioClip variedClip, Vector3 position)
    {
        if (instance == null)
            return;

        ClipVolumePitch clip = variedClip.GetRandomizedClip();
        instance.PlayAudioAtPosition(clip.clip, position, clip.volume, clip.pitch);
    }

    void PlayAudioAtPosition(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        AudioPlayer player = audioPlayerPool.Get(position);
        player.PlayClip(clip, volume, pitch);
        audioPlayerPool.ReturnAfterDelay(player, clip.length);
    }
}