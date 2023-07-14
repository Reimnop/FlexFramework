using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct SkinnedVertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public CameraData Camera { get; }
    public Matrix4[] Bones { get; }
    public TextureSamplerPair? Albedo { get; }
    public TextureSamplerPair? Metallic { get; }
    public TextureSamplerPair? Roughness { get; }
    public MaterialData Material { get; }

    public SkinnedVertexDrawData(
        IMeshView mesh, 
        Matrix4 transformation,
        CameraData camera, 
        Matrix4[] bones, 
        TextureSamplerPair? albedo, 
        TextureSamplerPair? metallic, 
        TextureSamplerPair? roughness,
        MaterialData material)
    {
        Mesh = mesh;
        Transformation = transformation;
        Camera = camera;
        Bones = bones;
        Albedo = albedo;
        Metallic = metallic;
        Roughness = roughness;
        Material = material;
    }
}