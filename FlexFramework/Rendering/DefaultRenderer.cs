using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using FlexFramework.Rendering.DefaultRenderingStrategies;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering;

public class DefaultRenderer : Renderer
{
    public const string OpaqueLayerName = "opaque";
    public const string TransparentLayerName = "transparent";
    public const string GuiLayerName = "gui";
    
    private Registry<string, List<IDrawData>> renderLayerRegistry = new Registry<string, List<IDrawData>>();
    private Dictionary<Type, RenderingStrategy> renderingStrategies = new Dictionary<Type, RenderingStrategy>();

    private GLStateManager glStateManager;
    private ShaderProgram unlitShader;

    private int opaqueLayerId;
    private int transparentLayerId;
    private int guiLayerId;

    public override void Init()
    {
        glStateManager = new GLStateManager();
        unlitShader = LoadProgram("unlit", "Assets/Shaders/unlit");
        
        // Register render layers
        opaqueLayerId = RegisterLayer(OpaqueLayerName);
        transparentLayerId = RegisterLayer(TransparentLayerName);
        guiLayerId = RegisterLayer(GuiLayerName);
        renderLayerRegistry.Freeze();
        
        // Register render strategies
        RegisterRenderingStrategy<VertexDrawData, VertexRenderStrategy>(unlitShader);
        RegisterRenderingStrategy<TexturedVertexDrawData, TexturedRenderStrategy>(unlitShader);
        RegisterRenderingStrategy<TextDrawData, TextRenderStrategy>(Engine);
        RegisterRenderingStrategy<CustomDrawData, CustomRenderStrategy>();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
    }

    private void RegisterRenderingStrategy<TDrawData, TStrategy>(params object?[]? args) 
        where TDrawData : IDrawData 
        where TStrategy : RenderingStrategy
    {
        TStrategy? strategy = Activator.CreateInstance(typeof(TStrategy), args) as TStrategy;
        if (strategy is null)
        {
            throw new ArgumentException();
        }
        renderingStrategies.Add(typeof(TDrawData), strategy);
    }

    private int RegisterLayer(string name)
    {
        return renderLayerRegistry.Register(name, () => new List<IDrawData>());
    }

    private ShaderProgram LoadProgram(string name, string path)
    {
        using Shader vertexShader = new Shader($"{path}-vs", File.ReadAllText($"{path}.vert"), ShaderType.VertexShader);
        using Shader fragmentShader = new Shader($"{path}-fs", File.ReadAllText($"{path}.frag"), ShaderType.FragmentShader);

        ShaderProgram program = new ShaderProgram(name);
        program.LinkShaders(vertexShader, fragmentShader);

        return program;
    }

    public override int GetLayerId(string name)
    {
        return renderLayerRegistry.GetId(name);
    }

    public override void EnqueueDrawData(int layerId, IDrawData drawData)
    {
        if (!renderLayerRegistry.HasKey(layerId))
        {
            return;
        }
        
        renderLayerRegistry[layerId].Add(drawData);
    }

    public override void Update(UpdateArgs args)
    {
    }

    public override void Render()
    {
        GL.Viewport(0, 0, Engine.ClientSize.X, Engine.ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        GL.Enable(EnableCap.Multisample);

        using TemporaryList<IDrawData> opaqueLayer = renderLayerRegistry[opaqueLayerId];
        using TemporaryList<IDrawData> transparentLayer = renderLayerRegistry[transparentLayerId];
        using TemporaryList<IDrawData> guiLayer = renderLayerRegistry[guiLayerId];
        
        GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(true);
        RenderLayer(opaqueLayer);
        
        GL.DepthMask(false);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        RenderLayer(transparentLayer);
        
        GL.DepthMask(true);
        
        GL.Disable(EnableCap.DepthTest);
        RenderLayer(guiLayer);
        GL.Disable(EnableCap.Blend);
    }

    private void RenderLayer(List<IDrawData> layer)
    {
        foreach (IDrawData drawData in layer)
        {
            RenderingStrategy strategy = renderingStrategies[drawData.GetType()];
            strategy.Draw(glStateManager, drawData);
        }
    }

    public override void Dispose()
    {
        unlitShader.Dispose();

        foreach (var (_, strategy) in renderingStrategies)
        {
            strategy.Dispose();
        }
    }
}