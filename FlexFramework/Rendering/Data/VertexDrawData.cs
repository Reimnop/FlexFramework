using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public class VertexDrawData : IDrawData
{
    public VertexArray VertexArray { get; set; }
    public int Count { get; set; }
    public Matrix4 Transformation { get; set; }

    public VertexDrawData(VertexArray vertexArray, int count, Matrix4 transformation)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
    }
}