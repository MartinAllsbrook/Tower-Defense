using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject defensePrefab;

    private bool waitingForClick = false;

    public void CreateDefense()
    {
        waitingForClick = true;
    }

    // Called by the new Input System when the Click action is triggered
    public void OnClick(InputAction.CallbackContext context)
    {
        // Only respond when the button is pressed (not released)
        if (context.performed && waitingForClick)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Instantiate(defensePrefab, mouseWorldPos, Quaternion.identity);
            waitingForClick = false;
        }
    }
}
