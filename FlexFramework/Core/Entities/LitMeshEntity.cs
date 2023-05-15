using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class LitMeshEntity : Entity, IRenderable
{
    public Mesh<LitVertex>? Mesh { get; set; }
    public Vector3 Albedo { get; set; } = Vector3.One; // White
    public float Metallic { get; set; } = 0.0f;
    public float Roughness { get; set; } = 1.0f;
    public Texture? AlbedoTexture { get; set; }
    public Texture? MetallicTexture { get; set; }
    public Texture? RoughnessTexture { get; set; }

    public void Render(RenderArgs args)
    {
        if (Mesh == null)
        {
            return;
        }
        
        CommandList commandList = args.CommandList;
        LayerType layerType = args.LayerType;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;

        MaterialData materialData = new MaterialData()
        {
            UseAlbedoTexture = AlbedoTexture != null,
            UseMetallicTexture = MetallicTexture != null,
            UseRoughnessTexture = RoughnessTexture != null,
            Albedo = Albedo,
            Metallic = Metallic,
            Roughness = Roughness
        };
        
        LitVertexDrawData vertexDrawData = new LitVertexDrawData(
            Mesh.ReadOnly, 
            matrixStack.GlobalTransformation, cameraData, 
            AlbedoTexture?.ReadOnly, MetallicTexture?.ReadOnly, RoughnessTexture?.ReadOnly,
            materialData);

        commandList.AddDrawData(layerType, vertexDrawData);
    }
}