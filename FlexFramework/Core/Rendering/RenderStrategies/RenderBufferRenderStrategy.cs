using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sampler = FlexFramework.Core.Rendering.Data.Sampler;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class RenderBufferRenderStrategy : RenderStrategy, IDisposable
{
    private readonly ShaderProgram program;
    private readonly MeshHandler meshHandler = new (
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.TexCoord0, 1),
            (VertexAttributeIntent.Color, 2)
        );

    private readonly Sampler sampler;

    public RenderBufferRenderStrategy()
    {
        using var vertexShader = new Shader("unlit_vert", File.ReadAllText("Assets/Shaders/unlit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("unlit_frag", File.ReadAllText("Assets/Shaders/unlit.frag"), ShaderType.FragmentShader);
        
        program = new ShaderProgram("unlit");
        program.LinkShaders(vertexShader, fragmentShader);
        
        sampler = new Sampler("render_buffer");
        sampler.Parameter(SamplerParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
        sampler.Parameter(SamplerParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        sampler.Parameter(SamplerParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
        sampler.Parameter(SamplerParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, CommandList commandList, IDrawData drawData)
    {
        var renderBufferDrawData = EnsureDrawDataType<RenderBufferDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(renderBufferDrawData.Mesh);
        var texture = renderBufferDrawData.RenderBuffer.Texture;

        glStateManager.UseProgram(program);
        glStateManager.BindVertexArray(mesh.VertexArray);
        glStateManager.BindTextureUnit(0, texture);
        glStateManager.BindSampler(0, sampler);

        var transformation = renderBufferDrawData.Transformation;
        GL.UniformMatrix4(program.GetUniformLocation("mvp"), true, ref transformation);
        GL.Uniform1(program.GetUniformLocation("hasTexture"), 1);

        GL.Uniform4(program.GetUniformLocation("color"), Color4.White);

        if (renderBufferDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(renderBufferDrawData.PrimitiveType, renderBufferDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(renderBufferDrawData.PrimitiveType, 0, renderBufferDrawData.Mesh.VerticesCount);
    }

    public void Dispose()
    {
        program.Dispose();
        meshHandler.Dispose();
        sampler.Dispose();
    }
}