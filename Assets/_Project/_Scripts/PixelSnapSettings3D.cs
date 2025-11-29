using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(
    fileName = "Pixel Snap Settings 3D",
    menuName = "Pixel Perfect 3D/Snap Settings")]
public class PixelSnapSettings3D : ScriptableObject
{
    [Header("Position")]
    [SerializeField] bool _snapPosition = true;

    [Header("Rotation")]
    [SerializeField] bool _snapRotation = true;

    [Range(1f, 360f)]
    [SerializeField] float _snapAngleIncrement = 45f;

    [SerializeField] bool _limitRotationUpdateRate;

    [Range(1, 60)]
    [SerializeField] int _rotationUpdatesPerSecond = 15;

    public bool SnapPosition => _snapPosition;
    public bool SnapRotation => _snapRotation;
    public float SnapAngleIncrement => _snapAngleIncrement;
    public bool LimitRotationUpdateRate => _limitRotationUpdateRate;
    public int RotationUpdatesPerSecond => _rotationUpdatesPerSecond;
}