using OpenTK.Mathematics;

namespace FlexFramework.Core.Util;

public class PerspectiveCamera : Camera
{
    public double Fov { get; set; } = MathHelper.DegreesToRadians(85.0);

    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4d view = Matrix4d.Invert(Matrix4d.CreateFromQuaternion(Rotation) * Matrix4d.CreateTranslation(Position));
        Matrix4d projection = Matrix4d.CreatePerspectiveFieldOfView(Fov, viewportSize.X / (double) viewportSize.Y, DepthNear, DepthFar);
        
        return new CameraData(view, projection);
    }
}