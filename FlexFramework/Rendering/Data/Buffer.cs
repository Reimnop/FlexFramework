using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering.Data;

public class Buffer : IGpuObject
{
    public int Handle { get; }
    public string Name { get; }

    public int SizeInBytes { get; private set; }

    public Buffer(string name)
    {
        Name = name;
        
        GL.CreateBuffers(1, out int handle);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, handle, name.Length, name);
        Handle = handle;
    }
    
    public void AllocateZero(int sizeInBytes)
    {
        SizeInBytes = sizeInBytes;
        GL.NamedBufferData(Handle, SizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
    }

    public void LoadData<T>(T[] data) where T : struct
    {
        SizeInBytes = data.Length * Unsafe.SizeOf<T>();
        GL.NamedBufferData(Handle, SizeInBytes, data, BufferUsageHint.DynamicDraw);
    }
    
    public void LoadDataPartial<T>(T[] data, int offsetInBytes) where T : struct
    {
        GL.NamedBufferSubData(Handle, (IntPtr) offsetInBytes, data.Length * Unsafe.SizeOf<T>(), data);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Handle);
    }
}