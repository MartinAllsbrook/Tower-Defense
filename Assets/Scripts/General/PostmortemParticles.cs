using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A MonoBehaviour script for controlling particle effects that occur after an entity's death or deactivation
/// </summary>
public class PostmortemParticles : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    [SerializeField] float autoDisableTime = 2f;

    Coroutine coroutine;

    void OnEnable()
    {
        coroutine = StartCoroutine(Play());
    }

    void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        particles.Stop();
    }

    IEnumerator Play()
    {
        particles.Play();
        yield return new WaitForSeconds(autoDisableTime);
        gameObject.SetActive(false);
        // yield return null;
    }
}