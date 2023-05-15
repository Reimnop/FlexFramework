using System.Diagnostics;
using FlexFramework.Core.Rendering.BackgroundRenderers;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.RenderStrategies;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Logging;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Color = System.Drawing.Color;

namespace FlexFramework.Core.Rendering.Renderers;

public class DefaultRenderer : Renderer, ILighting, IDisposable
{
    public Vector3 AmbientLight { get; set; } = Vector3.One * 0.4f;
    public DirectionalLight? DirectionalLight { get; set; }

    public override GpuInfo GpuInfo => gpuInfo;
    private GpuInfo gpuInfo = null!;
    
    private Dictionary<Type, RenderStrategy> renderStrategies = new Dictionary<Type, RenderStrategy>();

    private GLStateManager stateManager;
    private ShaderProgram skyboxShader;

    private int opaqueLayerId;
    private int alphaClipLayerId;
    private int transparentLayerId;
    private int guiLayerId;

    public override void Init()
    {
        stateManager = new GLStateManager();
        
        // Set GpuInfo
        string name = GL.GetString(StringName.Renderer);
        string vendor = GL.GetString(StringName.Vendor);
        string version = GL.GetString(StringName.Version);
        gpuInfo = new GpuInfo(name, vendor, version);
        
        // Init GL objects
        skyboxShader = LoadComputeProgram("skybox", "Assets/Shaders/Compute/skybox");

        // Register render strategies
        RegisterRenderStrategy<VertexDrawData>(new VertexRenderStrategy());
        RegisterRenderStrategy<LitVertexDrawData>(new LitVertexRenderStrategy(this));
        RegisterRenderStrategy<SkinnedVertexDrawData>(new SkinnedVertexRenderStrategy(this));
        RegisterRenderStrategy<TextDrawData>(new TextRenderStrategy(Engine));

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
        Engine.LogMessage(this, Severity.Info, null, $"Initializing render strategy [{strategy.GetType().Name}] for [{typeof(TDrawData).Name}]");
        renderStrategies.Add(typeof(TDrawData), strategy);
    }

    private ShaderProgram LoadComputeProgram(string name, string path)
    {
        using Shader shader = new Shader($"{name}-vs", File.ReadAllText($"{path}.comp"), ShaderType.ComputeShader);

        ShaderProgram program = new ShaderProgram(name);
        program.LinkShaders(shader);

        return program;
    }

    public override void Render(Vector2i size, CommandList commandList, IRenderBuffer renderBuffer)
    {
        // Ensure sizes always match
        if (renderBuffer.Size != size)
        {
            renderBuffer.Resize(size);
        }
        
        DefaultRenderBuffer drb = (DefaultRenderBuffer) renderBuffer;

        stateManager.BindFramebuffer(drb.WorldCapturer.FrameBuffer); // Bind world framebuffer

        GL.Viewport(0, 0, drb.WorldCapturer.Width, drb.WorldCapturer.Height);
        
        GL.ClearColor(ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        // Render background
        if (commandList.TryGetBackgroundRenderer(out var backgroundRenderer, out var backgroundCameraData))
        {
            backgroundRenderer.Render(this, stateManager, drb.WorldCapturer.ColorBuffer, backgroundCameraData);
        }
        
        // Opaque
        if (commandList.TryGetLayer(LayerType.Opaque, out var opaqueLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, false);
            stateManager.SetCapability(EnableCap.DepthTest, true);
            stateManager.SetCapability(EnableCap.CullFace, true);
            stateManager.SetCapability(EnableCap.Blend, false);
            stateManager.SetDepthMask(true);
            RenderLayer(opaqueLayer);
        }

        // Alpha clip
        if (commandList.TryGetLayer(LayerType.AlphaClip, out var alphaClipLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, false);
            stateManager.SetCapability(EnableCap.DepthTest, true);
            stateManager.SetCapability(EnableCap.CullFace, false);
            stateManager.SetCapability(EnableCap.Blend, false);
            stateManager.SetDepthMask(true);
            RenderLayer(alphaClipLayer);
        }

        // Transparent
        if (commandList.TryGetLayer(LayerType.Transparent, out var transparentLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, false);
            stateManager.SetCapability(EnableCap.DepthTest, true);
            stateManager.SetCapability(EnableCap.CullFace, false);
            stateManager.SetCapability(EnableCap.Blend, true);
            stateManager.SetDepthMask(false);
            RenderLayer(transparentLayer);
        }
        
        // Unbind
        stateManager.BindFramebuffer(null);

        // Post-process world framebuffer
        if (commandList.TryGetPostProcessors(out var postProcessors))
        {
            RunPostProcessors(postProcessors, drb.WorldCapturer.ColorBuffer);
        }

        stateManager.BindFramebuffer(drb.GuiCapturer.FrameBuffer); // Finish rendering world, bind gui framebuffer
        
        // Blit world framebuffer to gui framebuffer
        GL.ClearColor(Color.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BlitNamedFramebuffer(
            drb.WorldCapturer.FrameBuffer.Handle, drb.GuiCapturer.FrameBuffer.Handle, 
            0, 0, drb.Size.X, drb.Size.Y, 
            0, 0, drb.Size.X, drb.Size.Y, 
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

        // GUI
        if (commandList.TryGetLayer(LayerType.Gui, out var guiLayer))
        {
            stateManager.SetCapability(EnableCap.Multisample, true);
            stateManager.SetCapability(EnableCap.DepthTest, false);
            stateManager.SetCapability(EnableCap.CullFace, false);
            stateManager.SetCapability(EnableCap.Blend, true);
            stateManager.SetDepthMask(true);
            RenderLayer(guiLayer);
        }
        
        // Unbind
        stateManager.BindFramebuffer(null);
    }

    private void RunPostProcessors(IReadOnlyList<PostProcessor> postProcessors, Texture2D texture)
    {
        Vector2i size = new Vector2i(texture.Width, texture.Height);
        
        foreach (var processor in postProcessors)
        {
            if (processor.CurrentSize == Vector2i.Zero)
            {
                Engine.LogMessage(this, Severity.Info, null, $"Initializing post processor [{processor.GetType().Name}] with size {size}");
                processor.Init(size);
                return;
            }
            
            if (processor.CurrentSize != size)
            {
                Engine.LogMessage(this, Severity.Info, null, $"Resizing post processor [{processor.GetType().Name}] from {processor.CurrentSize} to {size}");
                processor.Resize(size);
            }
        }

        foreach (var processor in postProcessors)
        {
            processor.Process(stateManager, texture);
        }
    }

    private void RenderLayer(IReadOnlyList<IDrawData> layer)
    {
        foreach (var drawData in layer)
        {
            RenderStrategy strategy = renderStrategies[drawData.GetType()];
            strategy.Draw(stateManager, drawData);
        }
    }

    public void Dispose()
    {
        skyboxShader.Dispose();

        foreach (IDisposable strategy in renderStrategies.Values.OfType<IDisposable>())
        {
            strategy.Dispose();
        }
    }
}