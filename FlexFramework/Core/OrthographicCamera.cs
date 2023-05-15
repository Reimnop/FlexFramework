using OpenTK.Mathematics;

namespace FlexFramework.Core;

public class OrthographicCamera : Camera
{
    public float Size { get; set; } = 10.0f;

    public override float DepthNear { get; set; } = -1.0f;
    public override float DepthFar { get; set; } = 1.0f;

    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4 view = Matrix4.Invert(Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position));
        Matrix4 projection = Matrix4.CreateOrthographic(Size * (viewportSize.X / (float) viewportSize.Y), Size, DepthNear, DepthFar);
        
        return new CameraData(Position, Rotation, view, projection);
    }
}