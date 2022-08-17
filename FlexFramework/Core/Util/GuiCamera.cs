using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace UnifiedArrhythmicEditor.UI;

public class GuiCamera : OrthographicCamera
{
    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4d view = Matrix4d.Invert(Matrix4d.CreateFromQuaternion(Rotation) * Matrix4d.CreateTranslation(Position));
        Matrix4d projection = Matrix4d.CreateOrthographicOffCenter(0.0, Size * (viewportSize.X / (double) viewportSize.Y), 0.0, Size, DepthNear, DepthFar);
        
        return new CameraData(view, projection);
    }
}