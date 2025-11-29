using System.Collections.Generic;
using UnityEngine;

public class PixelSnapper3D : MonoBehaviour
{
    [SerializeField] bool _snapPosition = true;
    [SerializeField] bool _snapRotation = false;
    [SerializeField] float _rotationStepDegrees = 90f;

    public static IReadOnlyList<PixelSnapper3D> Instances => _instances;
    static readonly List<PixelSnapper3D> _instances = new List<PixelSnapper3D>();

    public bool SnapPositionEnabled
    {
        get => _snapPosition;
        set => _snapPosition = value;
    }

    public bool SnapRotationEnabled
    {
        get => _snapRotation;
        set => _snapRotation = value;
    }

    public float RotationStepDegrees
    {
        get => _rotationStepDegrees;
        set => _rotationStepDegrees = Mathf.Max(1f, value);
    }

    void OnEnable()
    {
        if (!_instances.Contains(this)) _instances.Add(this);
    }

    void OnDisable()
    {
        _instances.Remove(this);
    }

    void LateUpdate()
    {
        if (!PixelGrid3D.HasDefault) return;

        if (_snapPosition)
        {
            Vector3 snapped = PixelGrid3D.Default.SnapPosition(transform.position);
            transform.position = snapped;
        }

        if (_snapRotation)
        {
            Vector3 euler = transform.eulerAngles;
            float step = _rotationStepDegrees;
            euler.y = Mathf.Round(euler.y / step) * step;
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}