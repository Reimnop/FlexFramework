using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct LitVertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public CameraData Camera { get; }
    public TextureSamplerPair? Albedo { get; }
    public TextureSamplerPair? Metallic { get; }
    public TextureSamplerPair? Roughness { get; }
    public MaterialData Material { get; }

    public LitVertexDrawData(
        IMeshView mesh, 
        Matrix4 transformation, 
        CameraData camera, 
        TextureSamplerPair? albedo, 
        TextureSamplerPair? metallic, 
        TextureSamplerPair? roughness,
        MaterialData material)
    {
        Mesh = mesh;
        Transformation = transformation;
        Camera = camera;
        Albedo = albedo;
        Metallic = metallic;
        Roughness = roughness;
        Material = material;
    }
}