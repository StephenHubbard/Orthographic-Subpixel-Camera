using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelCamera3D : MonoBehaviour
{
    [SerializeField] Vector2Int _referenceResolution = new Vector2Int(320, 180);
    [SerializeField] int _pixelsPerUnit = 16;
    [SerializeField] float _zoom = 1f;
    [SerializeField] Transform _mount;
    [SerializeField] bool _snapToPixelGrid = true;

    public RenderTexture RenderTexture { get; private set; }
    public Vector2Int RenderResolution { get; private set; }
    public float UnitsPerPixel { get; private set; }
    public Vector2 PixelOffset { get; private set; }

    Camera _camera;

    public float Zoom
    {
        get => _zoom;
        set => _zoom = Mathf.Max(0.001f, value);
    }

    public Transform Mount
    {
        get => _mount;
        set => _mount = value;
    }

    public bool SnapToPixelGrid
    {
        get => _snapToPixelGrid;
        set => _snapToPixelGrid = value;
    }

    void OnValidate()
    {
        ClampSettings();
        EnsureCameraReference();
        UpdateRenderResolution();
        UpdateRenderTexture();
        UpdateCamera();
        UpdatePixelGrid();
        UpdateCameraTransform();
    }

    void Awake()
    {
        EnsureCameraReference();
        ClampSettings();
        UpdateRenderResolution();
        UpdateRenderTexture();
        UpdateCamera();
        UpdatePixelGrid();
        UpdateCameraTransform();
    }

    void Update()
    {
        EnsureCameraReference();
        UpdateRenderResolution();
        UpdateRenderTexture();
        UpdateCamera();
        UpdatePixelGrid();
        UpdateCameraTransform();
    }

    void EnsureCameraReference()
    {
        if (_camera == null) _camera = GetComponent<Camera>();
    }

    void ClampSettings()
    {
        if (_referenceResolution.x < 1) _referenceResolution.x = 1;
        if (_referenceResolution.y < 1) _referenceResolution.y = 1;
        if (_pixelsPerUnit < 1) _pixelsPerUnit = 1;
        if (_zoom < 0.001f) _zoom = 0.001f;
    }

    void UpdateRenderResolution()
    {
        RenderResolution = _referenceResolution;
    }

    void UpdateRenderTexture()
    {
        if (_camera == null) return;

        if (RenderTexture != null)
        {
            if (RenderTexture.width == RenderResolution.x && RenderTexture.height == RenderResolution.y)
            {
                if (_camera.targetTexture != RenderTexture) _camera.targetTexture = RenderTexture;
                return;
            }

            if (Application.isPlaying) Destroy(RenderTexture);
            else DestroyImmediate(RenderTexture);
            RenderTexture = null;
        }

        if (RenderResolution.x <= 0 || RenderResolution.y <= 0) return;

        RenderTexture = new RenderTexture(RenderResolution.x, RenderResolution.y, 24, RenderTextureFormat.Default);
        RenderTexture.filterMode = FilterMode.Point;
        RenderTexture.name = "PixelCamera3D_RT";

        _camera.targetTexture = RenderTexture;
    }

    void UpdateCamera()
    {
        if (_camera == null) return;

        _camera.orthographic = true;

        float targetWorldHeight = RenderResolution.y / (float)_pixelsPerUnit;
        float orthoSize = (targetWorldHeight * 0.5f) / _zoom;

        _camera.orthographicSize = orthoSize;
    }

    void UpdatePixelGrid()
    {
        if (_camera == null) return;
        if (RenderResolution.y <= 0) return;

        float worldHeight = _camera.orthographicSize * 2f;
        float unitsPerPixel = worldHeight / RenderResolution.y;

        UnitsPerPixel = unitsPerPixel;

        PixelGrid3D.SetDefault(unitsPerPixel, Quaternion.identity);
    }

    void UpdateCameraTransform()
    {
        if (_mount == null)
        {
            PixelOffset = Vector2.zero;
            return;
        }

        Vector3 targetPosition = _mount.position;

        if (!_snapToPixelGrid || !PixelGrid3D.HasDefault)
        {
            transform.position = targetPosition;
            PixelOffset = Vector2.zero;
            return;
        }

        Vector3 snappedPosition = PixelGrid3D.Default.SnapPosition(targetPosition);
        transform.position = snappedPosition;

        Vector3 deltaWorld = snappedPosition - targetPosition;

        if (UnitsPerPixel <= 0f)
        {
            PixelOffset = Vector2.zero;
            return;
        }

        Vector3 deltaLocal = transform.InverseTransformVector(deltaWorld);

        float offsetX = -deltaLocal.x / UnitsPerPixel;
        float offsetY = -deltaLocal.y / UnitsPerPixel;

        PixelOffset = new Vector2(offsetX, offsetY);
    }
}
