using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct SkinnedVertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public CameraData Camera { get; }
    public Matrix4[] Bones { get; }
    public ITextureView? AlbedoTexture { get; }
    public ITextureView? MetallicTexture { get; }
    public ITextureView? RoughnessTexture { get; }
    public MaterialData Material { get; }

    public SkinnedVertexDrawData(
        IMeshView mesh, 
        Matrix4 transformation,
        CameraData camera, 
        Matrix4[] bones, 
        ITextureView? albedoTexture, ITextureView? metallicTexture, ITextureView? roughnessTexture, 
        MaterialData material)
    {
        Mesh = mesh;
        Transformation = transformation;
        Camera = camera;
        Bones = bones;
        AlbedoTexture = albedoTexture;
        MetallicTexture = metallicTexture;
        RoughnessTexture = roughnessTexture;
        Material = material;
    }
}