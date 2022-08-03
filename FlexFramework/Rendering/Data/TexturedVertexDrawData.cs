using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public class TexturedVertexDrawData : IDrawData
{
    public VertexArray VertexArray { get; }
    public int Count { get; }
    public Matrix4 Transformation { get; set; }
    public Texture2D Texture { get; set; }

    public TexturedVertexDrawData(VertexArray vertexArray, int count, Matrix4 transformation, Texture2D texture)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
        Texture = texture;
    }
}