using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;

    private bool isPaused = false;

    public void GetTogglePauseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
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
