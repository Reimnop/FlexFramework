using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class VertexRenderStrategy : RenderStrategy
{
    private readonly ShaderProgram unlitShader;
    private readonly MeshHandler meshHandler = new (
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.TexCoord0, 1),
            (VertexAttributeIntent.Color, 2)
        );
    private readonly TextureHandler textureHandler = new();

    public VertexRenderStrategy()
    {
        using var vertexShader = new Shader("unlit-vert", File.ReadAllText("Assets/Shaders/unlit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("unlit-frag", File.ReadAllText("Assets/Shaders/unlit.frag"), ShaderType.FragmentShader);
        
        unlitShader = new ShaderProgram("unlit");
        unlitShader.LinkShaders(vertexShader, fragmentShader);
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
        textureHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        VertexDrawData vertexDrawData = EnsureDrawDataType<VertexDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(vertexDrawData.Mesh);
        Texture2D? texture = vertexDrawData.Texture != null ? textureHandler.GetTexture(vertexDrawData.Texture) : null;
        
        glStateManager.UseProgram(unlitShader);
        glStateManager.BindVertexArray(mesh.VertexArray);

        Matrix4 transformation = vertexDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.Uniform1(1, vertexDrawData.Texture == null ? 0 : 1);

        if (texture != null)
        {
            glStateManager.BindTextureUnit(0, texture);
        }

        GL.Uniform4(3, vertexDrawData.Color);

        if (vertexDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(vertexDrawData.PrimitiveType, vertexDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(vertexDrawData.PrimitiveType, 0, vertexDrawData.Mesh.VerticesCount);
    }
}