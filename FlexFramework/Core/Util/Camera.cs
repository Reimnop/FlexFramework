using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core;

public abstract class Camera
{
    public Vector3d Position { get; set; } = Vector3d.Zero;
    public Quaterniond Rotation { get; set; } = Quaterniond.Identity;
    
    public double DepthNear { get; set; } = 0.1;
    public double DepthFar { get; set; } = 100.0;

    public abstract CameraData GetCameraData(Vector2i viewportSize);
}