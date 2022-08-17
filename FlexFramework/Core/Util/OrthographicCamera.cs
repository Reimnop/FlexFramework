using OpenTK.Mathematics;

namespace FlexFramework.Core.Util;

public class OrthographicCamera : Camera
{
    public double Size { get; set; } = 10.0;

    public override double DepthNear { get; set; } = -1.0;
    public override double DepthFar { get; set; } = 1.0;

    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4d view = Matrix4d.Invert(Matrix4d.CreateFromQuaternion(Rotation) * Matrix4d.CreateTranslation(Position));
        Matrix4d projection = Matrix4d.CreateOrthographic(Size * (viewportSize.X / (double) viewportSize.Y), Size, DepthNear, DepthFar);
        
        return new CameraData(view, projection);
    }
}