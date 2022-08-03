using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public class TextDrawData : IDrawData
{
    public VertexArray VertexArray { get; }
    public int Count { get; set; }
    public Matrix4 Transformation { get; set; }

    public TextDrawData(VertexArray vertexArray, int count, Matrix4 transformation)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
    }
}