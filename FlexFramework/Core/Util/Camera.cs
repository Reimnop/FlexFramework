using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core;

public abstract class Camera
{
    public Vector3d Position { get; set; } = Vector3d.Zero;
    public Quaterniond Rotation { get; set; } = Quaterniond.Identity;
    
    public abstract double DepthNear { get; set; }
    public abstract double DepthFar { get; set; }

    public abstract CameraData GetCameraData(Vector2i viewportSize);
}