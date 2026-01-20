using UnityEngine;

class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip structureUnderAttackClip;
    AudioPlayer audioPlayer;

    void Awake()
    {
        audioPlayer = GetComponent<AudioPlayer>();
    }

    #region PublicSounds

    public void PlayStructureUnderAttackSound()
    {
        audioPlayer.PlayClip(structureUnderAttackClip);
    }

    #endregion
}