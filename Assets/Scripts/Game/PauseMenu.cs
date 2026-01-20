using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
