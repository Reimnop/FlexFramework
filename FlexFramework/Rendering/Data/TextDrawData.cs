using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public class TextDrawData : IDrawData
{
    public VertexArray VertexArray { get; set; }
    public int Count { get; set; }
    public Matrix4 Transformation { get; set; }
    public Color4 Color { get; set; }

    public TextDrawData(VertexArray vertexArray, int count, Matrix4 transformation, Color4 color)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
        Color = color;
    }
}