using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class FrameBuffer : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public FrameBuffer(string name)
    {
        Name = name;
        
        GL.CreateFramebuffers(1, out int handle);
        GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, handle, name.Length, name);

        Handle = handle;
    }

    public void Renderbuffer(FramebufferAttachment attachment, RenderBuffer renderBuffer)
    {
        GL.NamedFramebufferRenderbuffer(Handle, attachment, RenderbufferTarget.Renderbuffer, renderBuffer.Handle);
    }
    
    public void Texture(FramebufferAttachment attachment, Texture2D texture2D, int level = 0)
    {
        GL.NamedFramebufferTexture(Handle, attachment, texture2D.Handle, level);
    }
    
    public void DrawBuffers(params DrawBuffersEnum[] drawBuffers)
    {
        GL.NamedFramebufferDrawBuffers(Handle, drawBuffers.Length, drawBuffers);
    }

    public void DrawBuffer(DrawBufferMode drawBufferMode)
    {
        GL.NamedFramebufferDrawBuffer(Handle, drawBufferMode);
    }
    
    public void ReadBuffer(ReadBufferMode readBufferMode)
    {
        GL.NamedFramebufferReadBuffer(Handle, readBufferMode);
    }
    
    public void Dispose()
    {
        GL.DeleteFramebuffer(Handle);
    }
}