using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public class TexturedEntity : Entity, IRenderable
{
    public Texture2D Texture { get; set; }
    public bool MaintainAspectRatio { get; set; } = true;
    public Color4 Color { get; set; } = Color4.White;

    private Mesh<Vertex> quadMesh;
    private TexturedVertexDrawData vertexDrawData;

    public TexturedEntity(FlexFrameworkMain engine)
    {
        quadMesh = engine.PersistentResources.QuadMesh;
        vertexDrawData = new TexturedVertexDrawData(quadMesh.VertexArray, quadMesh.Count, Matrix4.Identity, null, Color);
    }
    
    public override void Update(UpdateArgs args)
    {
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Texture == null)
        {
            return;
        }

        vertexDrawData.Color = Color;
        
        matrixStack.Push();
        if (MaintainAspectRatio)
        {
            matrixStack.Scale(Texture.Width / (double) Texture.Height, 1.0, 1.0);
        }
        
        vertexDrawData.Transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        vertexDrawData.Texture = Texture;
        vertexDrawData.Color = Color;
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }
    
    public override void Dispose()
    {
    }
}