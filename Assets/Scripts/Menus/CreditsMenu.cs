using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName;

    public void LoadMainMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
