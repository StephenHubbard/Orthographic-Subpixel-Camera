using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class PixelCameraViewportContainer3D : MonoBehaviour
{
    [SerializeField] PixelCamera3D _pixelCamera;
    [SerializeField] PixelCameraViewport3D _viewport;

    RectTransform _rectTransform;

    public PixelCamera3D PixelCamera => _pixelCamera;
    public PixelCameraViewport3D Viewport => _viewport;

    void OnValidate()
    {
        EnsureReferences();
        UpdateLayout();
    }

    void Awake()
    {
        EnsureReferences();
        UpdateLayout();
    }

    void Update()
    {
        EnsureReferences();
        UpdateLayout();
    }

    void EnsureReferences()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

        if (_viewport == null)
            _viewport = GetComponentInChildren<PixelCameraViewport3D>();

        if (_pixelCamera == null && _viewport != null)
            _pixelCamera = _viewport.PixelCamera;
    }

    void UpdateLayout()
    {
        if (_rectTransform == null) return;
        if (_viewport == null) return;
        if (_pixelCamera == null) return;

        Vector2Int res = _pixelCamera.RenderResolution;
        if (res.x <= 0 || res.y <= 0) return;

        Rect rect = _rectTransform.rect;
        Vector2 containerSize = rect.size;

        if (containerSize.x <= 0f || containerSize.y <= 0f) return;

        float scaleX = containerSize.x / res.x;
        float scaleY = containerSize.y / res.y;
        float scale = Mathf.Floor(Mathf.Min(scaleX, scaleY));

        if (scale < 1f) scale = 1f;

        RectTransform viewportRect = _viewport.GetComponent<RectTransform>();
        viewportRect.localScale = new Vector3(scale, scale, 1f);
        viewportRect.anchoredPosition = Vector2.zero;
    }
}