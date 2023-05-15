using OpenTK.Mathematics;

namespace FlexFramework.Core;

public struct CameraData
{
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }
    public Matrix4 View { get; }
    public Matrix4 Projection { get; }

    public CameraData(Vector3 position, Quaternion rotation, Matrix4 view, Matrix4 projection)
    {
        Position = position;
        Rotation = rotation;
        View = view;
        Projection = projection;
    }
}