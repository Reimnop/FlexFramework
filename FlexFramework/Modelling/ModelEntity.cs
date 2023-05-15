using FlexFramework.Core;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Modelling;

public class ModelEntity : Entity, IRenderable
{
    public AnimationHandler AnimationHandler { get; }
    public Color4 Color { get; set; } = Color4.White;

    private readonly Model model;
    private readonly LitMeshEntity meshEntity;

    private float time = 0.0f;

    public ModelEntity(Model model)
    {
        this.model = model;
        
        meshEntity = new LitMeshEntity();
        AnimationHandler = new AnimationHandler(model);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;
        AnimationHandler.Update(time);
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

        matrixStack.Push();
        matrixStack.Transform(AnimationHandler.GetNodeTransform(modelNode));

        foreach (var modelMesh in modelNode.Meshes)
        {
            var material = model.Materials[modelMesh.MaterialIndex];
            
            meshEntity.Mesh = model.Meshes[modelMesh.MeshIndex];
            meshEntity.Albedo = material.Albedo;
            meshEntity.Metallic = material.Metallic;
            meshEntity.Roughness = material.Roughness;
            meshEntity.AlbedoTexture = material.AlbedoTexture;
            meshEntity.MetallicTexture = material.MetallicTexture;
            meshEntity.RoughnessTexture = material.RoughnessTexture;
            meshEntity.Render(args);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
        
        matrixStack.Pop();
    }
}