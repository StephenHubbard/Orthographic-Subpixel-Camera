using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PixelCameraController3D : MonoBehaviour
{
    [SerializeField] PixelCamera3D _pixelCamera;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _rotationSpeed = 120f;
    [SerializeField] float _zoomStep = 0.25f;
    [SerializeField] float _minZoom = 0.25f;
    [SerializeField] float _maxZoom = 4f;

    Vector2 _panInput;
    float _pendingTurn;
    float _pendingZoomSteps;

    public PixelCamera3D PixelCamera => _pixelCamera;

    void Awake()
    {
        if (_pixelCamera == null)
            _pixelCamera = GetComponentInChildren<PixelCamera3D>();
    }

    void Update()
    {
        Move();
        Rotate();
        Zoom();
    }

    void Move()
    {
        if (_panInput.sqrMagnitude <= 0f) return;

        Vector3 input = new Vector3(_panInput.x, 0f, _panInput.y);

        Vector3 forward = transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right   = transform.right;   right.y = 0f;   right.Normalize();

        Vector3 worldMove = forward * input.z + right * input.x;

        if (_pixelCamera != null)
            worldMove /= _pixelCamera.Zoom;

        transform.position += worldMove * _moveSpeed * Time.deltaTime;
    }

    void Rotate()
    {
        if (Mathf.Abs(_pendingTurn) <= 0f) return;

        float ang = _pendingTurn * _rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, ang, 0f, Space.World);

        _pendingTurn = 0f;
    }

    void Zoom()
    {
        if (_pixelCamera == null) return;
        if (Mathf.Abs(_pendingZoomSteps) <= 0f) return;

        float direction = Mathf.Sign(_pendingZoomSteps);
        float target = _pixelCamera.Zoom - direction * _zoomStep;
        target = Mathf.Clamp(target, _minZoom, _maxZoom);

        _pixelCamera.Zoom = target;
        _pendingZoomSteps -= direction;
    }

    // ───── Input Actions (these names MUST match exactly) ─────

    void OnPan(InputValue value)
    {
        _panInput = value.Get<Vector2>();
    }

    void OnZoom(InputValue value)
    {
        Vector2 scroll = value.Get<Vector2>();
        if (Mathf.Abs(scroll.y) > 0f)
            _pendingZoomSteps += Mathf.Sign(scroll.y);
    }

    void OnRotateLeft()
    {
        _pendingTurn -= 1f;
    }

    void OnRotateRight()
    {
        _pendingTurn += 1f;
    }

    // optional
    void OnToggleSnap()
    {
        Debug.Log("Toggle snap not implemented yet.");
    }

    // optional
    void OnSpawnObject()
    {
        Debug.Log("Spawn object not implemented yet.");
    }
}
