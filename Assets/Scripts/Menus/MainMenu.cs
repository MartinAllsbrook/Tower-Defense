using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string creditsSceneName;
    [SerializeField] private string gameSceneName;

    public void LoadCreditsScene()
    {
        if (!string.IsNullOrEmpty(creditsSceneName))
        {
            SceneManager.LoadScene(creditsSceneName);
        }
    }

    public void LoadGameScene()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
