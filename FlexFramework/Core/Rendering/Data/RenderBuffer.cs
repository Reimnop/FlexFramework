using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class RenderBuffer : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public RenderBuffer(string name, int width, int height, RenderbufferStorage format, int samples = 0)
    {
        Name = name;
        
        GL.CreateRenderbuffers(1, out int handle);

        if (samples > 0)
        {
            GL.NamedRenderbufferStorageMultisample(handle, samples, format, width, height);
        }
        else 
        {
            GL.NamedRenderbufferStorage(handle, format, width, height);
        }

        GL.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, handle, name.Length, name);

        Handle = handle;
    }

    public void Dispose()
    {
        GL.DeleteRenderbuffer(Handle);
    }
}