using System.Collections;
using UnityEngine;

/// <summary>
/// A MonoBehaviour script for controlling particle effects that occur after an entity's death or deactivation
/// </summary>
public class PostmortemParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    private bool isPlaying = false;

    private void OnEnable()
    {

        if (!isPlaying)
        {
            StartCoroutine(Play());
        }
        else {
            Debug.Log("PostmortemParticles is already playing.");
        }
    }

    IEnumerator Play()
    {
        Debug.Log("Playing PostmortemParticles");
        isPlaying = true;
        particles.Play();
        yield return new WaitForSeconds(particles.main.duration);
        isPlaying = false;
        Debug.Log("Disabling PostmortemParticles GameObject");
        gameObject.SetActive(false);
        // yield return null;
    }
}