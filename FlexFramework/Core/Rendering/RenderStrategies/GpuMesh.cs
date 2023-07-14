using FlexFramework.Core.Rendering.Data;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class GpuMesh : IDisposable
{
    public VertexArray VertexArray { get; }
    public Buffer VertexBuffer { get; }
    public Buffer? IndexBuffer { get; }
    
    public GpuMesh(VertexArray vertexArray, Buffer vertexBuffer, Buffer? indexBuffer)
    {
        VertexArray = vertexArray;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
    }
    
    public void Dispose()
    {
        VertexArray.Dispose();
        VertexBuffer.Dispose();
        IndexBuffer?.Dispose();
    }
}