using OpenTK.Mathematics;

namespace FlexFramework.Core;

public class GuiCamera : Camera
{
    public override float DepthNear { get; set; } = -10.0f;
    public override float DepthFar { get; set; } = 10.0f;
    
    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4 view = Matrix4.Invert(Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position));
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, viewportSize.X, viewportSize.Y, 0.0f, DepthNear, DepthFar);
        
        return new CameraData(Position, Rotation, view, projection);
    }
}