using System.Collections.Generic;
using UnityEngine;

public class PixelSnapper3D : MonoBehaviour
{
    [SerializeField] bool _snapPosition = true;
    [SerializeField] bool _snapRotation = false;
    [SerializeField] float _rotationStepDegrees = 90f;
    [SerializeField] PixelSnapSettings3D _snapSettings;

    public static IReadOnlyList<PixelSnapper3D> Instances => _instances;
    static readonly List<PixelSnapper3D> _instances = new List<PixelSnapper3D>();

    float _rotationSnapTimer;

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

    public PixelSnapSettings3D SnapSettings
    {
        get => _snapSettings;
        set => _snapSettings = value;
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

        bool snapPosition = _snapPosition;
        bool snapRotation = _snapRotation;
        float rotationStep = _rotationStepDegrees;
        bool limitRate = false;
        int updatesPerSecond = 0;

        if (_snapSettings != null)
        {
            snapPosition = _snapSettings.SnapPosition;
            snapRotation = _snapSettings.SnapRotation;
            rotationStep = _snapSettings.SnapAngleIncrement;
            limitRate = _snapSettings.LimitRotationUpdateRate;
            updatesPerSecond = _snapSettings.RotationUpdatesPerSecond;
        }

        if (snapPosition)
        {
            Vector3 snapped = PixelGrid3D.Default.SnapPosition(transform.position);
            transform.position = snapped;
        }

        if (!snapRotation) return;

        bool updateRotation = true;

        if (limitRate)
        {
            updateRotation = false;
            _rotationSnapTimer -= Time.deltaTime;
            if (_rotationSnapTimer <= 0f)
            {
                int clampedHz = Mathf.Max(1, updatesPerSecond);
                _rotationSnapTimer += 1f / clampedHz;
                updateRotation = true;
            }
        }

        if (!updateRotation) return;

        float step = Mathf.Max(1f, rotationStep);
        Vector3 euler = transform.eulerAngles;
        euler.y = Mathf.Round(euler.y / step) * step;
        transform.rotation = Quaternion.Euler(euler);
    }
}
