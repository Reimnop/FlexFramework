using FlexFramework.Core.Data;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.Rendering.Data;

public struct TextDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public Color4 Color { get; }
    public float DistanceRange { get; }

    public TextDrawData(IMeshView mesh, Matrix4 transformation, Color4 color, float distanceRange)
    {
        Mesh = mesh;
        Transformation = transformation;
        Color = color;
        DistanceRange = distanceRange;
    }
}