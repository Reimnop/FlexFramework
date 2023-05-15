using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct LitVertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public CameraData Camera { get; }
    public ITextureView? AlbedoTexture { get; }
    public ITextureView? MetallicTexture { get; }
    public ITextureView? RoughnessTexture { get; }
    public MaterialData Material { get; }

    public LitVertexDrawData(
        IMeshView mesh, 
        Matrix4 transformation, CameraData camera, 
        ITextureView? albedoTexture, ITextureView? metallicTexture, ITextureView? roughnessTexture, 
        MaterialData material)
    {
        Mesh = mesh;
        Transformation = transformation;
        Camera = camera;
        AlbedoTexture = albedoTexture;
        MetallicTexture = metallicTexture;
        RoughnessTexture = roughnessTexture;
        Material = material;
    }
}