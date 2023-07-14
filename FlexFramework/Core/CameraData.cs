using OpenTK.Mathematics;

namespace FlexFramework.Core;

/// <summary>
/// Container for information about the camera
/// </summary>
public struct CameraData
{
    /// <summary>
    /// Position of the camera in world space
    /// </summary>
    public Vector3 Position { get; }
    
    /// <summary>
    /// Rotation of the camera in world space
    /// </summary>
    public Quaternion Rotation { get; }

    /// <summary>
    /// Matrix for transforming from world space to view space
    /// </summary>
    public Matrix4 View { get; }
    
    /// <summary>
    /// Matrix for transforming from view space to clip space
    /// </summary>
    public Matrix4 Projection { get; }

    public CameraData(Vector3 position, Quaternion rotation, Matrix4 view, Matrix4 projection)
    {
        Position = position;
        Rotation = rotation;
        View = view;
        Projection = projection;
    }
}