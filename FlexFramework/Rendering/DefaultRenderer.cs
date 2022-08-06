using System.Drawing;
using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering;

public class DefaultRenderer : Renderer
{
    public const string OpaqueLayerName = "opaque";
    public const string TransparentLayerName = "transparent";
    public const string GuiLayerName = "gui";
    
    private Registry<List<IDrawData>> renderLayerRegistry = new Registry<List<IDrawData>>();

    private ShaderProgram unlitShader;
    private ShaderProgram textShader;

    private int opaqueLayerId;
    private int transparentLayerId;
    private int guiLayerId;

    public override void Init()
    {
        opaqueLayerId = renderLayerRegistry.Register(OpaqueLayerName, () => new List<IDrawData>());
        transparentLayerId = renderLayerRegistry.Register(TransparentLayerName, () => new List<IDrawData>());
        guiLayerId = renderLayerRegistry.Register(GuiLayerName, () => new List<IDrawData>());
        renderLayerRegistry.Freeze();

        unlitShader = LoadProgram("unlit", "Assets/Shaders/unlit");
        textShader = LoadProgram("text", "Assets/Shaders/text");
        
        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
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
            if (drawData is VertexDrawData vertexDrawData)
            {
                GL.UseProgram(unlitShader.Handle);
                GL.BindVertexArray(vertexDrawData.VertexArray.Handle);

                Matrix4 transformation = vertexDrawData.Transformation;
                GL.UniformMatrix4(0, true, ref transformation);
                GL.Uniform1(1, 0);

                GL.DrawArrays(PrimitiveType.Triangles, 0, vertexDrawData.Count);
            } 
            else if (drawData is TexturedVertexDrawData texturedVertexDrawData)
            {
                GL.UseProgram(unlitShader.Handle);
                GL.BindVertexArray(texturedVertexDrawData.VertexArray.Handle);

                Matrix4 transformation = texturedVertexDrawData.Transformation;
                GL.UniformMatrix4(0, true, ref transformation);
                GL.Uniform1(1, 1);
                GL.Uniform1(2, 0);
                GL.BindTextureUnit(0, texturedVertexDrawData.Texture.Handle);
                
                GL.DrawArrays(PrimitiveType.Triangles, 0, texturedVertexDrawData.Count);
            }
            else if (drawData is TextDrawData textDrawData)
            {
                GL.UseProgram(textShader.Handle);
                
                GL.BindVertexArray(textDrawData.VertexArray.Handle);

                Matrix4 transformation = textDrawData.Transformation;
                GL.UniformMatrix4(0, true, ref transformation);
                
                for (int i = 0; i < Engine.TextResources.FontTextures.Length; i++)
                {
                    GL.Uniform1(i + 1, i);
                    GL.BindTextureUnit(i, Engine.TextResources.FontTextures[i].Handle);
                }
                
                GL.DrawArrays(PrimitiveType.Triangles, 0, textDrawData.Count);
            }
        }
    }

    public override void Dispose()
    {
        unlitShader.Dispose();
    }
}