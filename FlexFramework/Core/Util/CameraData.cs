using OpenTK.Mathematics;

namespace FlexFramework.Core.Util;

public struct CameraData
{
    public Matrix4d View { get; }
    public Matrix4d Projection { get; }

    public CameraData(Matrix4d view, Matrix4d projection)
    {
        View = view;
        Projection = projection;
    }
}