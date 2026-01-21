using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }
    private @InputSystem_Actions controls;

    public event Action OnClick = delegate { };
    public event Action OnRelease = delegate { };
    public event Action OnCancel = delegate { };
    public event Action OnPause = delegate { }; // This is kind of a fake input 
    public event Action<float> OnZoomCamera = delegate { };
    public event Action<Vector2> OnMoveCamera = delegate { };

    void Awake()
    {
        Debug.Log("Initializing InputReader Singleton");
        if (Instance == null) Instance = this;
        controls = new @InputSystem_Actions();

        LinkEvents();
    }

    void OnEnable() 
    {
        controls.Enable();
    }

    void OnDisable() 
    {
        controls.Disable();
    }

    void LinkEvents()
    {
        Debug.Log("Linking InputReader Events");

        // Clicking 
        controls.Player.Click.performed += _ctx => OnClick.Invoke();
        controls.Player.Click.canceled += _ctx => OnRelease.Invoke();
        
        // Camera Movement
        controls.Player.MoveCamera.performed += ctx => OnMoveCamera.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.MoveCamera.canceled += ctx => OnMoveCamera.Invoke(ctx.ReadValue<Vector2>()); // Should be (0,0) on cancel
    
        // Cancel (Pause)
        controls.Player.Cancel.performed += _ctx => OnCancel.Invoke();

        // Zoom Camera
        controls.Player.Zoom.performed += ctx => OnZoomCamera.Invoke(ctx.ReadValue<float>());
        controls.Player.Zoom.canceled += ctx => OnZoomCamera.Invoke(0f); // Zero zoom on cancel
    }

    public void SkipCancel()
    {
        Debug.Log("Skipping Cancel Event");
        OnPause.Invoke();
    }
}
