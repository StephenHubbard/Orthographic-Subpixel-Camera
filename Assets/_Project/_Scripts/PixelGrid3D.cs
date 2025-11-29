using UnityEngine;

public struct PixelGrid3D
{
    public static PixelGrid3D Default { get; private set; }
    public static bool HasDefault => Default.IsValid;

    public float UnitsPerPixel { get; }
    public Quaternion Rotation { get; }
    public bool IsValid => UnitsPerPixel > 0f;

    public PixelGrid3D(float unitsPerPixel, Quaternion rotation)
    {
        UnitsPerPixel = unitsPerPixel;
        Rotation = rotation;
    }

    public static void SetDefault(float unitsPerPixel, Quaternion rotation)
    {
        Default = new PixelGrid3D(unitsPerPixel, rotation);
    }

    public Vector3 WorldToGrid(Vector3 worldPosition)
    {
        Vector3 local = Quaternion.Inverse(Rotation) * worldPosition;
        return local / UnitsPerPixel;
    }

    public Vector3 GridToWorld(Vector3 gridPosition)
    {
        Vector3 local = gridPosition * UnitsPerPixel;
        return Rotation * local;
    }

    public Vector3 SnapPosition(Vector3 worldPosition)
    {
        Vector3 grid = WorldToGrid(worldPosition);
        grid.x = Mathf.Round(grid.x);
        grid.y = Mathf.Round(grid.y);
        grid.z = Mathf.Round(grid.z);
        return GridToWorld(grid);
    }
}