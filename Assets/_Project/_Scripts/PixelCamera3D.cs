using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelCamera3D : MonoBehaviour
{
    [SerializeField] Vector2Int _referenceResolution = new Vector2Int(320, 180);
    [SerializeField] int _pixelsPerUnit = 16;
    [SerializeField] float _zoom = 1f;
    
    public RenderTexture RenderTexture { get; private set; }
    public Vector2Int RenderResolution { get; private set; }

    Camera _camera;

    void OnValidate()
    {
        ClampSettings();
        EnsureCameraReference();
        UpdateRenderResolution();
        UpdateRenderTexture();
        UpdateCamera();
    }

    void Awake()
    {
        EnsureCameraReference();
        ClampSettings();
        UpdateRenderResolution();
        UpdateRenderTexture();
        UpdateCamera();
    }

    void Update()
    {
        EnsureCameraReference();
        UpdateRenderResolution();
        UpdateRenderTexture();
        UpdateCamera();
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
}
