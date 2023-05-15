using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Modelling;

// TODO: Implement support for non-zero mesh origins
// Alright, I can't fix this
// Please don't use non-zero mesh origins
public class SkinnedModelEntity : Entity, IRenderable
{
    public AnimationHandler AnimationHandler { get; }

    private readonly Model model;
    private readonly Matrix4[] boneMatrices;
    private readonly Matrix4 globalInverseTransform;

    private readonly MatrixStack boneMatrixStack = new();

    private float time = 0.0f;

    public SkinnedModelEntity(Model model)
    {
        this.model = model;
        boneMatrices = new Matrix4[model.Bones.Count];
        globalInverseTransform = Matrix4.Invert(model.RootNode.Value.Transform);
        AnimationHandler = new AnimationHandler(model);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;
        AnimationHandler.Update(time);
        
        CalculateBoneMatricesRecursively(model.RootNode, boneMatrixStack);
    }
    
    private void CalculateBoneMatricesRecursively(ImmutableNode<ModelNode> node, MatrixStack matrixStack)
    {
        var modelNode = node.Value;
        
        matrixStack.Push();
        matrixStack.Transform(AnimationHandler.GetNodeTransform(modelNode));

        if (model.BoneIndexMap.TryGetValue(modelNode.Name, out int boneIndex))
        {
            boneMatrices[boneIndex] = model.Bones[boneIndex].Offset * matrixStack.GlobalTransformation * globalInverseTransform;
        }

        foreach (var child in node.Children)
        {
            CalculateBoneMatricesRecursively(child, matrixStack);
        }
        
        matrixStack.Pop();
    }

    public void Render(RenderArgs args)
    {
        RenderModelRecursively(model.RootNode, args);
    }
    
    // more recursion bullshit
    private void RenderModelRecursively(ImmutableNode<ModelNode> node, RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        var modelNode = node.Value;

        foreach (var modelMesh in modelNode.Meshes)
        {
            var material = model.Materials[modelMesh.MaterialIndex];

            CommandList commandList = args.CommandList;
            LayerType layerType = args.LayerType;
            CameraData cameraData = args.CameraData;
            
            MaterialData materialData = new MaterialData()
            {
                UseAlbedoTexture = material.AlbedoTexture != null,
                UseMetallicTexture = material.MetallicTexture != null,
                UseRoughnessTexture = material.RoughnessTexture != null,
                Albedo = material.Albedo,
                Metallic = material.Metallic,
                Roughness = material.Roughness
            };
        
            SkinnedVertexDrawData vertexDrawData = new SkinnedVertexDrawData(
                model.SkinnedMeshes[modelMesh.MeshIndex].ReadOnly, 
                matrixStack.GlobalTransformation, 
                cameraData, 
                boneMatrices,
                material.AlbedoTexture?.ReadOnly, material.MetallicTexture?.ReadOnly, material.RoughnessTexture?.ReadOnly,
                materialData);
        
            commandList.AddDrawData(layerType, vertexDrawData);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
    }
}