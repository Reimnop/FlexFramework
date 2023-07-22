using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sampler = FlexFramework.Core.Rendering.Data.Sampler;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class TextRenderStrategy : RenderStrategy, IDisposable
{
    private readonly ShaderProgram program;
    private readonly Sampler sampler;
    private readonly MeshHandler meshHandler = new(
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.TexCoord0, 1)
    );
    private readonly TextureHandler textureHandler = new();
    
    public TextRenderStrategy()
    {
        using var vertexShader = new Shader("text_vert", File.ReadAllText("Assets/Shaders/Vertex/text.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("text_frag", File.ReadAllText("Assets/Shaders/Fragment/text.frag"), ShaderType.FragmentShader);
        
        program = new ShaderProgram("text");
        program.LinkShaders(vertexShader, fragmentShader);
        
        sampler = new Sampler("text_sampler");
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
        var textDrawData = EnsureDrawDataType<TextDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(textDrawData.Mesh);
        var texture = textureHandler.GetTexture(textDrawData.FontAtlas);
        
        glStateManager.UseProgram(program);
        glStateManager.BindVertexArray(mesh.VertexArray);
        glStateManager.BindTextureUnit(0, texture);
        glStateManager.BindSampler(0, sampler);

        Matrix4 transformation = textDrawData.Transformation;
        GL.UniformMatrix4(program.GetUniformLocation("mvp"), true, ref transformation);
        GL.Uniform4(program.GetUniformLocation("overlayColor"), textDrawData.Color);
        GL.Uniform1(program.GetUniformLocation("distanceRange"), textDrawData.DistanceRange);
        
        if (textDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, textDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, textDrawData.Mesh.VerticesCount);
    }

    public void Dispose()
    {
        program.Dispose();
        sampler.Dispose();
        meshHandler.Dispose();
        textureHandler.Dispose();
    }
}