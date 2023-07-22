using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.RenderStrategies;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Util.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Half = OpenTK.Mathematics.Half;

namespace FlexFramework.Core.Rendering.Renderers;

public class DefaultRenderer : Renderer, IDisposable
{
    public override GpuInfo GpuInfo { get; }

    private readonly Dictionary<Type, RenderStrategy> renderStrategies = new();

    private readonly GLStateManager stateManager;
    
    // Framebuffers for copying and blitting
    private readonly FrameBuffer readFrameBuffer;
    private readonly FrameBuffer drawFrameBuffer;

    private readonly ILogger logger;

    public DefaultRenderer(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<DefaultRenderer>();
        stateManager = new GLStateManager();
        
        // Set GpuInfo
        var name = GL.GetString(StringName.Renderer);
        var vendor = GL.GetString(StringName.Vendor);
        var version = GL.GetString(StringName.Version);
        GpuInfo = new GpuInfo(name, vendor, version);
        
        // Create framebuffers
        readFrameBuffer = new FrameBuffer("read");
        drawFrameBuffer = new FrameBuffer("draw");

        // Register render strategies
        RegisterRenderStrategy<RenderBufferDrawData>(new RenderBufferRenderStrategy());
        RegisterRenderStrategy<VertexDrawData>(new VertexRenderStrategy());
        RegisterRenderStrategy<LitVertexDrawData>(new LitVertexRenderStrategy());
        RegisterRenderStrategy<SkinnedVertexDrawData>(new SkinnedVertexRenderStrategy());
        RegisterRenderStrategy<TextDrawData>(new TextRenderStrategy());

        // Set GL modes
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public override IRenderBuffer CreateRenderBuffer(Vector2i size)
    {
        return new DefaultRenderBuffer(size);
    }

    public override void Update(UpdateArgs args)
    {
        // Update strategies
        foreach (var strategy in renderStrategies.Values)
        {
            strategy.Update(args);
        }
    }

    private void RegisterRenderStrategy<TDrawData>(RenderStrategy strategy) where TDrawData : IDrawData
    {
        logger.LogInfo($"Initializing render strategy [{strategy.GetType().Name}] for [{typeof(TDrawData).Name}]");
        renderStrategies.Add(typeof(TDrawData), strategy);
    }

    public override void Render(Vector2i size, CommandList commandList, IRenderBuffer renderBuffer)
    {
        // Ensure sizes always match
        if (renderBuffer.Size != size)
        {
            renderBuffer.Resize(size);
        }
        
        var drb = (DefaultRenderBuffer) renderBuffer;

        // Bind world framebuffer
        stateManager.BindFramebuffer(drb.WorldFrameBuffer);

        GL.Viewport(0, 0, drb.Size.X, drb.Size.Y);

        // Clear screen
        GL.ClearColor(commandList.GetClearColor());
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Render background
        if (commandList.TryGetBackgroundRenderer(out var backgroundRenderer, out var backgroundCameraData))
            backgroundRenderer.Render(commandList, stateManager, drb, backgroundCameraData);

        // Opaque
        if (commandList.TryGetLayer(LayerType.Opaque, out var opaqueLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, false);
            stateManager.SetCapability(EnableCap.DepthTest, true);
            stateManager.SetCapability(EnableCap.CullFace, true);
            stateManager.SetCapability(EnableCap.Blend, false);
            stateManager.SetDepthMask(true);
            RenderLayer(commandList, opaqueLayer);
        }

        // Alpha clip
        if (commandList.TryGetLayer(LayerType.AlphaClip, out var alphaClipLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, false);
            stateManager.SetCapability(EnableCap.DepthTest, true);
            stateManager.SetCapability(EnableCap.CullFace, false);
            stateManager.SetCapability(EnableCap.Blend, false);
            stateManager.SetDepthMask(true);
            RenderLayer(commandList, alphaClipLayer);
        }

        // Transparent
        if (commandList.TryGetLayer(LayerType.Transparent, out var transparentLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, false);
            stateManager.SetCapability(EnableCap.DepthTest, true);
            stateManager.SetCapability(EnableCap.CullFace, false);
            stateManager.SetCapability(EnableCap.Blend, true);
            stateManager.SetDepthMask(false);
            RenderLayer(commandList, transparentLayer);
        }
        
        // Unbind
        stateManager.BindFramebuffer(null);
        
        // Copy world color to world final
        GL.CopyImageSubData(
            drb.WorldColor.Handle, ImageTarget.Texture2D, 0, 0, 0, 0,
            drb.WorldFinal.Handle, ImageTarget.Texture2D, 0, 0, 0, 0,
            drb.Size.X, drb.Size.Y, 1);

        // Post-process world framebuffer
        if (commandList.TryGetPostProcessors(out var postProcessors))
        {
            RunPostProcessors(postProcessors, drb, drb.WorldFinal);
        }
        
        // Copy world final to gui color
        CopyImageFbo(drb.WorldFinal, drb.GuiColor, drb.Size);

        stateManager.BindFramebuffer(drb.GuiFrameBuffer); // Bind gui framebuffer

        // GUI
        if (commandList.TryGetLayer(LayerType.Gui, out var guiLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, true);
            stateManager.SetCapability(EnableCap.DepthTest, false);
            stateManager.SetCapability(EnableCap.CullFace, false);
            stateManager.SetCapability(EnableCap.Blend, true);
            stateManager.SetDepthMask(true);
            RenderLayer(commandList, guiLayer);
        }
        
        // Finally, copy gui color to final texture
        CopyImageFbo(drb.GuiColor, drb.GuiFinal, drb.Size);
        
        // Unbind
        stateManager.BindFramebuffer(null);
    }

    private void RunPostProcessors(IReadOnlyList<PostProcessor> postProcessors, IRenderBuffer renderBuffer, Texture2D texture)
    {
        var size = new Vector2i(texture.Width, texture.Height);
        
        foreach (var processor in postProcessors)
        {
            if (!processor.Initialized)
            {
                logger.LogInfo($"Initializing post processor [{processor.GetType().Name}] with size {size}");
                processor.Init(size);
            }

            if (processor.CurrentSize == size)
                continue;
            
            logger.LogInfo($"Resizing post processor [{processor.GetType().Name}] from {processor.CurrentSize} to {size}");
            processor.Init(size);
        }

        foreach (var processor in postProcessors)
        {
            processor.Process(stateManager, renderBuffer, texture);
        }
    }

    private void RenderLayer(CommandList commandList, IReadOnlyList<IDrawData> layer)
    {
        foreach (var drawData in layer)
        {
            var strategy = renderStrategies[drawData.GetType()];
            strategy.Draw(stateManager, commandList, drawData);
        }
    }
    
    private void CopyImageFbo(Texture2D source, Texture2D destination, Vector2i size)
    {
        // Bind source and destination
        readFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, source);
        drawFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, destination);
        
        // Copy image
        GL.BlitNamedFramebuffer(
            readFrameBuffer.Handle, drawFrameBuffer.Handle, 
            0, 0, size.X, size.Y, 
            0, 0, size.X, size.Y, 
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
    }

    public void Dispose()
    {
        foreach (IDisposable strategy in renderStrategies.Values.OfType<IDisposable>())
        {
            strategy.Dispose();
        }
        
        readFrameBuffer.Dispose();
        drawFrameBuffer.Dispose();
    }
}