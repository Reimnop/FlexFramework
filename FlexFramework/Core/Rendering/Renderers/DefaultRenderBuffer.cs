using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Renderers;

public class DefaultRenderBuffer : IRenderBuffer, IDisposable
{
    public Vector2i Size { get; private set; }

    public ScreenCapturer WorldCapturer { get; private set; }
    public ScreenCapturer GuiCapturer { get; private set; }

    public DefaultRenderBuffer(Vector2i size)
    {
        WorldCapturer = new ScreenCapturer("world", size.X, size.Y);
        GuiCapturer = new ScreenCapturer("gui", size.X, size.Y, false, 4);
    }

    public void Resize(Vector2i size)
    {
        if (Size != size)
        {
            Size = size;

            WorldCapturer.Dispose();
            GuiCapturer.Dispose();
            WorldCapturer = new ScreenCapturer("world", size.X, size.Y);
            GuiCapturer = new ScreenCapturer("gui", size.X, size.Y, false, 4);
        }
    }

    public void BlitToBackBuffer(Vector2i size)
    {
        GL.BlitNamedFramebuffer(GuiCapturer.FrameBuffer.Handle, 0, 
            0, 0, Size.X, Size.Y, 
            0, 0, size.X, size.Y,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
    }

    public void Dispose()
    {
        WorldCapturer.Dispose();
        GuiCapturer.Dispose();
    }
}