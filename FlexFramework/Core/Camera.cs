using FlexFramework.Core;
using OpenTK.Mathematics;

namespace FlexFramework.Core;

public abstract class Camera
{
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    
    public abstract float DepthNear { get; set; }
    public abstract float DepthFar { get; set; }

    public abstract CameraData GetCameraData(Vector2i viewportSize);
}