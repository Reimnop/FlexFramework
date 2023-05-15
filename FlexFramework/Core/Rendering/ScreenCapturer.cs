using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering;

// TODO: Nuke this class
public class ScreenCapturer : IDisposable
{
    public int Width { get; }
    public int Height { get; }

    public FrameBuffer FrameBuffer { get; }
    public Texture2D ColorBuffer { get; }
    public RenderBuffer? DepthBuffer { get; }

    public ScreenCapturer(string name, int width, int height, bool useDepth = true, int samples = 0)
    {
        Width = width;
        Height = height;

        ColorBuffer = new Texture2D($"{name}-color", width, height, SizedInternalFormat.Rgba16f, samples);
        
        if (samples == 0)
        {
            ColorBuffer.SetMinFilter(TextureMinFilter.Linear);
            ColorBuffer.SetMagFilter(TextureMagFilter.Linear);
            ColorBuffer.SetWrapS(TextureWrapMode.ClampToEdge);
            ColorBuffer.SetWrapT(TextureWrapMode.ClampToEdge);
        }

        FrameBuffer = new FrameBuffer(name);
        FrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, ColorBuffer);
        
        if (useDepth)
        {
            DepthBuffer = new RenderBuffer($"{name}-depth", width, height, RenderbufferStorage.DepthComponent32f, samples);
            FrameBuffer.Renderbuffer(FramebufferAttachment.DepthAttachment, DepthBuffer);
        }
    }

    public void Dispose()
    {
        FrameBuffer.Dispose();
        ColorBuffer.Dispose();
        DepthBuffer?.Dispose();
    }
}