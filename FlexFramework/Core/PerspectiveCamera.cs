using OpenTK.Mathematics;

namespace FlexFramework.Core;

public class PerspectiveCamera : Camera
{
    public float Fov { get; set; } = MathHelper.DegreesToRadians(85.0f);

    public override float DepthNear { get; set; } = 0.1f;
    public override float DepthFar { get; set; } = 100.0f;

    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4 view = Matrix4.Invert(Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position));
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(Fov, viewportSize.X / (float) viewportSize.Y, DepthNear, DepthFar);
        
        return new CameraData(Position, Rotation, view, projection);
    }
}