using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class VertexRenderStrategy : RenderStrategy, IDisposable
{
    private readonly ShaderProgram program;
    private readonly MeshHandler meshHandler = new (
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.TexCoord0, 1),
            (VertexAttributeIntent.Color, 2)
        );
    private readonly TextureHandler textureHandler = new();
    private readonly SamplerHandler samplerHandler = new();

    public VertexRenderStrategy()
    {
        using var vertexShader = new Shader("unlit_vert", File.ReadAllText("Assets/Shaders/Vertex/unlit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("unlit_frag", File.ReadAllText("Assets/Shaders/Fragment/unlit.frag"), ShaderType.FragmentShader);
        
        program = new ShaderProgram("unlit");
        program.LinkShaders(vertexShader, fragmentShader);
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
        textureHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, CommandList commandList, IDrawData drawData)
    {
        var vertexDrawData = EnsureDrawDataType<VertexDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(vertexDrawData.Mesh);
        var texture = vertexDrawData.Texture;

        glStateManager.UseProgram(program);
        glStateManager.BindVertexArray(mesh.VertexArray);

        var transformation = vertexDrawData.Transformation;
        GL.UniformMatrix4(program.GetUniformLocation("mvp"), true, ref transformation);
        GL.Uniform1(program.GetUniformLocation("hasTexture"), vertexDrawData.Texture == null ? 0 : 1);

        if (texture.HasValue)
        {
            var tex = textureHandler.GetTexture(texture.Value.Texture);
            var sampler = samplerHandler.GetSampler(texture.Value.Sampler);
            glStateManager.BindTextureUnit(0, tex);
            glStateManager.BindSampler(0, sampler);
        }

        GL.Uniform4(program.GetUniformLocation("color"), vertexDrawData.Color);

        if (vertexDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(vertexDrawData.PrimitiveType, vertexDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(vertexDrawData.PrimitiveType, 0, vertexDrawData.Mesh.VerticesCount);
    }

    public void Dispose()
    {
        program.Dispose();
        meshHandler.Dispose();
        textureHandler.Dispose();
        samplerHandler.Dispose();
    }
}