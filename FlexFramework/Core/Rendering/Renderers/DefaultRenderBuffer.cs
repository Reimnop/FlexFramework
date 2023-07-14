using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Renderers;

public class DefaultRenderBuffer : IRenderBuffer, IGBuffer, IDisposable
{
    public Vector2i Size { get; private set; }

    public FrameBuffer WorldFrameBuffer { get; }
    public FrameBuffer GuiFrameBuffer { get; }
    
    public Texture2D Texture => guiFinal;

    // Texture attachments
    public Texture2D WorldFinal => worldFinal;
    public Texture2D WorldColor => worldColor;
    public Texture2D WorldNormal => worldNormal;
    public Texture2D WorldPosition => worldPosition;
    public Texture2D WorldDepth => worldDepth;
    public Texture2D GuiColor => guiColor;
    public Texture2D GuiFinal => guiFinal;
    
    private Texture2D worldFinal;
    private Texture2D worldColor;
    private Texture2D worldNormal;
    private Texture2D worldPosition;
    private Texture2D worldDepth;
    private Texture2D guiColor;
    private Texture2D guiFinal; // Same as guiColor, but without multisampling

    public DefaultRenderBuffer(Vector2i size)
    {
        // Initialize framebuffers
        WorldFrameBuffer = new FrameBuffer("world");
        WorldFrameBuffer.DrawBuffers(DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2);
        
        GuiFrameBuffer = new FrameBuffer("gui");
        
        // Initialize textures
        CreateTextures(size, out worldFinal, out worldColor, out worldNormal, out worldPosition, out worldDepth, out guiColor, out guiFinal);
        
        // Attach textures to framebuffers
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, worldColor);
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment1, worldNormal);
        WorldFrameBuffer.Texture(FramebufferAttachment.DepthAttachment, worldDepth);
        GuiFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, guiColor);
    }

    public void Resize(Vector2i size)
    {
        if (Size == size) 
            return;
        
        Size = size;

        // Dispose old textures
        worldColor.Dispose();
        worldNormal.Dispose();
        worldDepth.Dispose();
        guiColor.Dispose();
            
        // Initialize textures
        CreateTextures(size, out worldFinal, out worldColor, out worldNormal, out worldPosition, out worldDepth, out guiColor, out guiFinal);
        
        // Attach textures to framebuffers
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, worldColor);
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment1, worldNormal);
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment2, worldPosition);
        WorldFrameBuffer.Texture(FramebufferAttachment.DepthAttachment, worldDepth);
        GuiFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, guiColor);
    }

    private static void CreateTextures(Vector2i size, 
        out Texture2D worldFinal, 
        out Texture2D worldColor, 
        out Texture2D worldNormal, 
        out Texture2D worldPosition, 
        out Texture2D worldDepth, 
        out Texture2D guiColor,
        out Texture2D guiFinal)
    {
        worldFinal = new Texture2D("world_final", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldColor = new Texture2D("world_color", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldNormal = new Texture2D("world_normal", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldPosition = new Texture2D("world_position", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldDepth = new Texture2D("world_depth", size.X, size.Y, SizedInternalFormat.DepthComponent32f);
        guiColor = new Texture2D("gui_color", size.X, size.Y, SizedInternalFormat.Rgba16f, samples: 4);
        guiFinal = new Texture2D("gui_final", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }

    public void Dispose()
    {
        WorldFrameBuffer.Dispose();
        GuiFrameBuffer.Dispose();
        worldColor.Dispose();
        worldNormal.Dispose();
        worldPosition.Dispose();
        worldDepth.Dispose();
        guiColor.Dispose();
        guiFinal.Dispose();
    }
}