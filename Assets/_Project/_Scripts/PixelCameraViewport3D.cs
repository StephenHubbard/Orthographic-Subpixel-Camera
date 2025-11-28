using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImage))]
public class PixelCameraViewport3D : MonoBehaviour
{
    [SerializeField] PixelCamera3D _pixelCamera;

    RawImage _rawImage;

    public PixelCamera3D PixelCamera => _pixelCamera;

    void OnValidate()
    {
        EnsureReferences();
        UpdateTexture();
        UpdateSize();
    }

    void Awake()
    {
        EnsureReferences();
        UpdateTexture();
        UpdateSize();
    }

    void Update()
    {
        EnsureReferences();
        UpdateTexture();
        UpdateSize();
    }

    void EnsureReferences()
    {
        if (_rawImage == null) _rawImage = GetComponent<RawImage>();
    }

    void UpdateTexture()
    {
        if (_rawImage == null) return;
        if (_pixelCamera == null) return;

        if (_rawImage.texture != _pixelCamera.RenderTexture)
            _rawImage.texture = _pixelCamera.RenderTexture;
    }

    void UpdateSize()
    {
        if (_rawImage == null) return;
        if (_pixelCamera == null) return;

        Vector2Int res = _pixelCamera.RenderResolution;
        if (res.x <= 0 || res.y <= 0) return;

        _rawImage.rectTransform.sizeDelta = new Vector2(res.x, res.y);
    }
}