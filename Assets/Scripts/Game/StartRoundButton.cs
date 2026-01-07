using UnityEngine;
using UnityEngine.UI;

public class StartRoundButton : MonoBehaviour
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        GameController gameController = FindFirstObjectByType<GameController>();
        gameController.OnBasePlaced += Enable;
        gameController.OnRoundEnd += Enable;

        button.onClick.AddListener(() =>
        {
            gameController.StartRound();
            Disable();
        });
    }

    void Enable()
    {
        button.interactable = true;
    }

    void Disable()
    {
        button.interactable = false;
    }
}
