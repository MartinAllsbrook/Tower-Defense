using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;

    private bool isPaused = false;

    void OnEnable()
    {
        if (InputReader.Instance == null)
        {
            StartCoroutine(SubscribeWhenReady());
            return;
        }

        InputReader.Instance.OnPause += TogglePause;   
    }

    IEnumerator SubscribeWhenReady()
    {
        while (InputReader.Instance == null)
            yield return null;

        InputReader.Instance.OnPause += TogglePause;   
    }

    void OnDisable()
    {
        InputReader.Instance.OnPause -= TogglePause;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
            PauseGame();
        else
            UnpauseGame();
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
    }

    void UnpauseGame()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }
}
