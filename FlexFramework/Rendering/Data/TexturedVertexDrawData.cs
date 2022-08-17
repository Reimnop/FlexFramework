using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public class TexturedVertexDrawData : IDrawData
{
    public VertexArray VertexArray { get; set; }
    public int Count { get; set; }
    public Matrix4 Transformation { get; set; }
    public Texture2D Texture { get; set; }
    public Color4 Color { get; set; }

    public TexturedVertexDrawData(VertexArray vertexArray, int count, Matrix4 transformation, Texture2D texture, Color4 color)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
        Texture = texture;
        Color = color;
    }
}