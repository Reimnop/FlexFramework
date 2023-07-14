using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sampler = FlexFramework.Core.Data.Sampler;
using TextureSampler = FlexFramework.Core.Data.TextureSampler;

namespace FlexFramework.Core.Entities;

public class ImageEntity : Entity, IRenderable
{
    public TextureSampler Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;

    public ImageEntity(TextureSampler texture)
    {
        Texture = texture;
    }

    public void Render(RenderArgs args)
    {
        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;
        var transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        var vertexDrawData = new VertexDrawData(DefaultAssets.QuadMesh.ReadOnly, transformation, (TextureSamplerPair) Texture, Color, PrimitiveType.Triangles);
        commandList.AddDrawData(layerType, vertexDrawData);
    }
}