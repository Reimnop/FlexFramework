using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class SkinnedVertexRenderStrategy : RenderStrategy, IDisposable
{
    private readonly ILighting lighting;
    private readonly ShaderProgram skinnedShader;
    
    private readonly MeshHandler meshHandler = new(
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.Normal, 1),
            (VertexAttributeIntent.TexCoord0, 2),
            (VertexAttributeIntent.Color, 3),
            (VertexAttributeIntent.BoneIndex, 4),
            (VertexAttributeIntent.BoneWeight, 5)
        );
    private readonly TextureHandler textureHandler = new();
    private readonly Buffer materialBuffer;
    
    public SkinnedVertexRenderStrategy(ILighting lighting)
    {
        this.lighting = lighting;
        
        // Shader init
        using var vertexShader = new Shader("skinned-vs", File.ReadAllText("Assets/Shaders/skinned.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("skinned-fs", File.ReadAllText("Assets/Shaders/lit.frag"), ShaderType.FragmentShader);

        skinnedShader = new ShaderProgram("skinned");
        skinnedShader.LinkShaders(vertexShader, fragmentShader);
        
        // Buffer init
        materialBuffer = new Buffer("material");
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
        textureHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        var vertexDrawData = EnsureDrawDataType<SkinnedVertexDrawData>(drawData);
        var material = vertexDrawData.Material;
        
        materialBuffer.LoadData(material);

        var mesh = meshHandler.GetMesh(vertexDrawData.Mesh);
        var albedoTexture = vertexDrawData.AlbedoTexture != null
            ? textureHandler.GetTexture(vertexDrawData.AlbedoTexture)
            : null;
        var metallicTexture = vertexDrawData.MetallicTexture != null
            ? textureHandler.GetTexture(vertexDrawData.MetallicTexture)
            : null;
        var roughnessTexture = vertexDrawData.RoughnessTexture != null
            ? textureHandler.GetTexture(vertexDrawData.RoughnessTexture)
            : null;
        
        glStateManager.UseProgram(skinnedShader);
        glStateManager.BindVertexArray(mesh.VertexArray);
        
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, materialBuffer.Handle);

        Matrix4 mvp = vertexDrawData.Transformation * vertexDrawData.Camera.View * vertexDrawData.Camera.Projection;
        Matrix4 model = vertexDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref mvp);
        GL.UniformMatrix4(1, true, ref model);

        if (albedoTexture != null)
        {
            GL.Uniform1(2, 0);
            glStateManager.BindTextureUnit(0, albedoTexture);
        }
        
        if (metallicTexture != null)
        {
            GL.Uniform1(3, 1);
            glStateManager.BindTextureUnit(1, metallicTexture);
        }

        if (roughnessTexture != null)
        {
            GL.Uniform1(4, 2);
            glStateManager.BindTextureUnit(2, roughnessTexture);
        }

        GL.Uniform3(5, lighting.AmbientLight); 

        if (lighting.DirectionalLight.HasValue)
        {
            GL.Uniform3(6, lighting.DirectionalLight.Value.Direction);
            GL.Uniform3(7, lighting.DirectionalLight.Value.Color * lighting.DirectionalLight.Value.Intensity);
        }
        
        GL.Uniform3(8, vertexDrawData.Camera.Position);

        for (int i = 0; i < vertexDrawData.Bones.Length; i++)
        {
            Matrix4 bone = vertexDrawData.Bones[i];
            GL.UniformMatrix4(9 + i, true, ref bone);
        }

        if (vertexDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexDrawData.Mesh.VerticesCount);
    }

    public void Dispose()
    {
        skinnedShader.Dispose();
    }
}