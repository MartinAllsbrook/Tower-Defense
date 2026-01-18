using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] musicTracks;
    
    private List<int> shuffledIndices = new List<int>();
    private int currentTrackIndex = 0;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (musicTracks != null && musicTracks.Length > 0)
        {
            ShufflePlaylist();
            PlayNextTrack();
        }
    }

    private void Update()
    {
        // Check if current track has finished playing
        if (!audioSource.isPlaying && musicTracks != null && musicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }

    private void ShufflePlaylist()
    {
        shuffledIndices.Clear();
        
        // Add all indices to the list
        for (int i = 0; i < musicTracks.Length; i++)
        {
            shuffledIndices.Add(i);
        }

        // Shuffle using Fisher-Yates algorithm
        for (int i = shuffledIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[randomIndex];
            shuffledIndices[randomIndex] = temp;
        }

        currentTrackIndex = 0;
    }

    private void PlayNextTrack()
    {
        if (musicTracks == null || musicTracks.Length == 0)
            return;

        // If we've reached the end of the shuffled playlist, reshuffle
        if (currentTrackIndex >= shuffledIndices.Count)
        {
            ShufflePlaylist();
        }

        // Get the next track from the shuffled list
        int trackIndex = shuffledIndices[currentTrackIndex];
        AudioClip nextTrack = musicTracks[trackIndex];

        if (nextTrack != null)
        {
            audioSource.clip = nextTrack;
            audioSource.Play();
        }

        currentTrackIndex++;
    }
}
