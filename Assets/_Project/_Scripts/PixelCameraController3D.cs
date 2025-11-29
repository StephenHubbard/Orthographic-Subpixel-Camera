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
    [SerializeField] bool _snapEnabled = true;

    Vector2 _panInput;
    float _rotateInput;
    float _pendingZoomSteps;

    public PixelCamera3D PixelCamera => _pixelCamera;
    public bool SnapEnabled => _snapEnabled;

    void Awake()
    {
        if (_pixelCamera == null)
            _pixelCamera = GetComponentInChildren<PixelCamera3D>();

        ApplySnapState();
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
        Vector3 right = transform.right; right.y = 0f; right.Normalize();

        Vector3 worldMove = forward * input.z + right * input.x;

        if (_pixelCamera != null)
            worldMove /= _pixelCamera.Zoom;

        transform.position += worldMove * _moveSpeed * Time.deltaTime;
    }

    void Rotate()
    {
        if (Mathf.Abs(_rotateInput) <= 0f) return;

        float ang = _rotateInput * _rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, ang, 0f, Space.World);
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

    void ApplySnapState()
    {
        if (_pixelCamera != null)
            _pixelCamera.SnapToPixelGrid = _snapEnabled;

        foreach (PixelSnapper3D snapper in PixelSnapper3D.Instances)
        {
            if (snapper == null) continue;
            snapper.SnapPositionEnabled = _snapEnabled;
        }
    }

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

    void OnRotate(InputValue value)
    {
        _rotateInput = value.Get<float>();
    }

    void OnToggleSnap()
    {
        _snapEnabled = !_snapEnabled;
        ApplySnapState();
    }

    void OnSpawnObject()
    {
    }
}
