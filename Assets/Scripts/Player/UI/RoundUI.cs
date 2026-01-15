using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI roundText;

    GameController gameController;

    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();
        gameController.OnBasePlaced += OnRoundReady;
        gameController.OnRoundEnd += OnRoundReady;

        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        gameController.StartRound();
        Disable();
    }

    void OnRoundReady(int roundNumber)
    {
        roundText.text = $"Round {roundNumber + 1}";
        button.interactable = true;
    }

    void Disable()
    {
        button.interactable = false;
    }
}
