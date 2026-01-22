using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 15f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float edgePanBorder = 10f;
    [SerializeField] private bool useEdgePanning = false;
    
    private Camera cam;
    private Vector2 moveInput;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void OnEnable()
    {
        if (InputReader.Instance == null)
        {
            StartCoroutine(SubscribeWhenReady());
            return;
        }

        InputReader.Instance.OnMoveCamera += SetMoveInput;
        InputReader.Instance.OnZoomCamera += SetZoomInput;
    }

    IEnumerator SubscribeWhenReady()
    {
        while (InputReader.Instance == null)
            yield return null;

        InputReader.Instance.OnMoveCamera += SetMoveInput;
        InputReader.Instance.OnZoomCamera += SetZoomInput;
    }

    void OnDisable()
    {
        InputReader.Instance.OnMoveCamera -= SetMoveInput;
        InputReader.Instance.OnZoomCamera -= SetZoomInput;
    }

    void SetMoveInput(Vector2 input) => moveInput = input;
    void SetZoomInput(float increment) => AdjustZoom(increment);

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);

        // Optional edge panning with mouse
        if (useEdgePanning)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            if (mousePos.x >= Screen.width - edgePanBorder)
                movement.x += 1f;
            if (mousePos.x <= edgePanBorder)
                movement.x -= 1f;
            if (mousePos.y >= Screen.height - edgePanBorder)
                movement.y += 1f;
            if (mousePos.y <= edgePanBorder)
                movement.y -= 1f;
        }

        transform.position += movement * panSpeed * Time.deltaTime;
    }

    private void AdjustZoom(float increment)
    {
        if (cam != null && cam.orthographic)
        {
            float newSize = cam.orthographicSize - increment * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }
}
