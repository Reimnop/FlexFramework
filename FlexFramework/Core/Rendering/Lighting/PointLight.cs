using OpenTK.Mathematics;

namespace FlexFramework;
public struct PointLight
{
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }
    public float Intensity { get; set; }

    public PointLight(Vector3 position, Vector3 color, float intensity)
    {
        Position = position;
        Color = color;
        Intensity = intensity;
    }
}
