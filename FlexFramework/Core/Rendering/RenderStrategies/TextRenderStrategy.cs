using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class TextRenderStrategy : RenderStrategy, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly TextAssets textAssets;
    private readonly ShaderProgram textShader;
    
    private readonly MeshHandler meshHandler = new(
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.Color, 1),
            (VertexAttributeIntent.TexCoord0, 2),
            (VertexAttributeIntent.TexCoord1, 3)
        );
    
    public TextRenderStrategy(FlexFrameworkMain engine)
    {
        this.engine = engine;
        var textAssetLocation = engine.DefaultAssets.TextAssets;
        textAssets = engine.ResourceRegistry.GetResource(textAssetLocation);

        using var vertexShader = new Shader("text-vert", File.ReadAllText("Assets/Shaders/text.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("text-frag", File.ReadAllText("Assets/Shaders/text.frag"), ShaderType.FragmentShader);
        
        textShader = new ShaderProgram("text");
        textShader.LinkShaders(vertexShader, fragmentShader);
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        TextDrawData textDrawData = EnsureDrawDataType<TextDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(textDrawData.Mesh);
        
        glStateManager.UseProgram(textShader);
        glStateManager.BindVertexArray(mesh.VertexArray);

        Matrix4 transformation = textDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
                
        for (int i = 0; i < textAssets.AtlasTextures.Count; i++)
        {
            GL.Uniform1(i + 1, i);
            glStateManager.BindTextureUnit(i, textAssets.AtlasTextures[i]);
        }

        GL.Uniform4(17, textDrawData.Color);
        GL.Uniform1(18, textDrawData.DistanceRange);
        
        if (textDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, textDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, textDrawData.Mesh.VerticesCount);
    }

    public void Dispose()
    {
        textShader.Dispose();
    }
}