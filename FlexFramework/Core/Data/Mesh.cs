using System.Runtime.CompilerServices;
using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Rendering.Data.Buffer;

namespace FlexFramework.Core.Data;

public class Mesh<T> : IDisposable where T : struct
{
    public VertexArray VertexArray { get; }
    public Buffer VertexBuffer { get; }
    public int Count { get; private set; }

    private int currentAttribIndex = 0;
    
    public Mesh(string name)
    {
        VertexBuffer = new Buffer($"{name}-vtx");
        VertexArray = new VertexArray(name);
    }

    public Mesh(string name, T[] vertices)
    {
        VertexBuffer = new Buffer($"{name}-vtx");
        VertexArray = new VertexArray(name);
        
        LoadData(vertices);
    }

    public void LoadData(T[] vertices)
    {
        Count = vertices.Length;
        VertexBuffer.LoadData(vertices);
    }

    public void Attribute(int size, int offset, VertexAttribType vertexAttribType, bool normalized)
    {
        VertexArray.VertexBuffer(VertexBuffer, currentAttribIndex, currentAttribIndex, size, offset, vertexAttribType, normalized, Unsafe.SizeOf<T>());
        currentAttribIndex++;
    }
    
    public void Attribute(int size, int offset, VertexAttribIntegerType vertexAttribIntegerType)
    {
        VertexArray.VertexBuffer(VertexBuffer, currentAttribIndex, currentAttribIndex, size, offset, vertexAttribIntegerType, Unsafe.SizeOf<T>());
        currentAttribIndex++;
    }

    public void Dispose()
    {
        VertexArray.Dispose();
        VertexBuffer.Dispose();
    }
}