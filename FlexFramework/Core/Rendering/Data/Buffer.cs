using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class Buffer : IGpuObject, IDisposable
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

    public unsafe void LoadData<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        SizeInBytes = data.Length * Unsafe.SizeOf<T>();

        fixed (T* ptr = data)
        {
            GL.NamedBufferData(Handle, SizeInBytes, (IntPtr) ptr, BufferUsageHint.DynamicDraw);
        }
    }

    public unsafe void LoadData<T>(T data) where T : unmanaged
    {
        SizeInBytes = Unsafe.SizeOf<T>();

        T* ptr = stackalloc T[1];
        ptr[0] = data;
        GL.NamedBufferData(Handle, SizeInBytes, (IntPtr) ptr, BufferUsageHint.DynamicDraw);
    }

    public unsafe void LoadDataPartial<T>(ReadOnlySpan<T> data, int offsetInBytes) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            GL.NamedBufferSubData(Handle, (IntPtr) offsetInBytes, data.Length * Unsafe.SizeOf<T>(), (IntPtr) ptr);
        }
    }

    public void Dispose()
    {
        GL.DeleteBuffer(Handle);
    }
}