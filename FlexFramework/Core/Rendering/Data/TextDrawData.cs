using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct TextDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public ITextureView FontAtlas { get; }
    public Matrix4 Transformation { get; }
    public Color4 Color { get; }
    public float DistanceRange { get; }

    public TextDrawData(IMeshView mesh, ITextureView fontAtlas, Matrix4 transformation, Color4 color, float distanceRange)
    {
        Mesh = mesh;
        FontAtlas = fontAtlas;
        Transformation = transformation;
        Color = color;
        DistanceRange = distanceRange;
    }
}