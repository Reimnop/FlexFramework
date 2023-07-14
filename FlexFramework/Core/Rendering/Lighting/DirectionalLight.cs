using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Lighting;

public struct DirectionalLight
{
    public static DirectionalLight None => new(Vector3.Zero, Vector3.Zero, 0);

    public Vector3 Direction { get; set; }
    public Vector3 Color { get; set; }
    public float Intensity { get; set; }
    
    public DirectionalLight(Vector3 direction, Vector3 color, float intensity)
    {
        Direction = direction;
        Color = color;
        Intensity = intensity;
    }
}