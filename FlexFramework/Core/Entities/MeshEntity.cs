using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TextureSampler = FlexFramework.Core.Data.TextureSampler;

namespace FlexFramework.Core.Entities;

public class MeshEntity : Entity, IRenderable
{
    public Mesh<Vertex> Mesh { get; set; }
    public TextureSampler? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;
    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Triangles;

    public MeshEntity(Mesh<Vertex> mesh)
    {
        Mesh = mesh;
    }

    public void Render(RenderArgs args)
    {
        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;

        var transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        var vertexDrawData = new VertexDrawData(Mesh.ReadOnly, transformation, (TextureSamplerPair?) Texture, Color, PrimitiveType);
        
        commandList.AddDrawData(layerType, vertexDrawData);
    }
}