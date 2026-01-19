using System;
using UnityEngine;

/// <summary>
/// Base class for all tutorial panels. Handles show/hide logic and events.
/// Extend this class for specific panel types (info, tooltip, highlight, etc.)
/// </summary>
public abstract class TutorialPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float fadeInDuration = 0.3f;
    [SerializeField] protected float fadeOutDuration = 0.3f;
    [SerializeField] protected bool blockRaycastsWhenVisible = true;

    // Events
    public event Action<TutorialPanel> OnPanelShown;
    public event Action<TutorialPanel> OnPanelClosed;

    protected bool isVisible = false;
    private Coroutine fadeCoroutine;

    protected virtual void Awake()
    {
        // Auto-find CanvasGroup if not assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Start hidden
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void OnDestroy()
    {
        // Clean up any running coroutines
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }

    public virtual void Show()
    {
        if (isVisible) return;

        gameObject.SetActive(true);
        isVisible = true;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeIn());
        OnPanelShown?.Invoke(this);
    }

    public virtual void Hide()
    {
        if (!isVisible) return;

        isVisible = false;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOut());
        OnPanelClosed?.Invoke(this);
    }

    private System.Collections.IEnumerator FadeIn()
    {
        canvasGroup.interactable = true;
        if (blockRaycastsWhenVisible)
        {
            canvasGroup.blocksRaycasts = true;
        }

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public bool IsVisible() => isVisible;

    // Override these in derived classes for custom behavior
    protected virtual void OnShowComplete() { }
    protected virtual void OnHideComplete() { }
}
