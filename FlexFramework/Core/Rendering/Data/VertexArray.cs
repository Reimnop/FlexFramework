using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class VertexArray : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public VertexArray(string name)
    {
        Name = name;
        
        GL.CreateVertexArrays(1, out int handle);
        GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, handle, name.Length, name);

        Handle = handle;
    }
    
    public void VertexBuffer(Buffer buffer, int bindingIndex, int attribIndex, int size, int offset, VertexAttribType vertexAttribType, bool normalized, int stride)
    {
        GL.EnableVertexArrayAttrib(Handle, attribIndex);
        GL.VertexArrayVertexBuffer(Handle, bindingIndex, buffer.Handle, (IntPtr) offset, stride);
        GL.VertexArrayAttribFormat(Handle, attribIndex, size, vertexAttribType, normalized, 0);
        GL.VertexArrayAttribBinding(Handle, attribIndex, bindingIndex);
    }

    public void VertexBuffer(Buffer buffer, int bindingIndex, int attribIndex, int size, int offset, VertexAttribIntegerType vertexAttribIntegerType, int stride)
    {
        GL.EnableVertexArrayAttrib(Handle, attribIndex);
        GL.VertexArrayVertexBuffer(Handle, bindingIndex, buffer.Handle, (IntPtr) offset, stride);
        GL.VertexArrayAttribIFormat(Handle, attribIndex, size, vertexAttribIntegerType, 0);
        GL.VertexArrayAttribBinding(Handle, attribIndex, bindingIndex);
    }

    public void ElementBuffer(Buffer buffer)
    {
        GL.VertexArrayElementBuffer(Handle, buffer.Handle);
    }
    
    public void Dispose()
    {
        GL.DeleteVertexArray(Handle);
    }
}