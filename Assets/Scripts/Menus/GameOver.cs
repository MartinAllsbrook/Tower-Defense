using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundsSurvivedText;

    void Start()
    {
        int roundsSurvived = PlayerPrefs.GetInt("RoundsSurvived", 0);
        roundsSurvivedText.text = "Rounds Survived: " + roundsSurvived.ToString();
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
