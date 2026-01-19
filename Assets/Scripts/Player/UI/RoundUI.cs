using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI roundText;

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);

        GameController.OnBasePlaced += OnRoundReady;
        GameController.OnRoundEnd += OnRoundReady;
    }

    void OnDisable()
    {
        button.onClick.AddListener(OnClick);

        GameController.OnBasePlaced -= OnRoundReady;
        GameController.OnRoundEnd -= OnRoundReady;
    }

    void OnClick()
    {
        GameController.StartRound();
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
