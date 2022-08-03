using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public class TexturedEntity : Entity, IRenderable, ITransformable
{
    public Vector3d Position { get; set; } = Vector3d.Zero;
    public Vector3d Scale { get; set; } = Vector3d.One;
    public Quaterniond Rotation { get; set; } = Quaterniond.Identity;

    public Texture2D Texture { get; set; }
    public bool MaintainAspectRatio { get; set; } = true;

    private Mesh<Vertex> quadMesh;
    private TexturedVertexDrawData vertexDrawData;

    public TexturedEntity(FlexFrameworkMain engine)
    {
        quadMesh = engine.PersistentResources.QuadMesh;
        vertexDrawData = new TexturedVertexDrawData(quadMesh.VertexArray, quadMesh.Count, Matrix4.Identity, null);
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

        matrixStack.Push();
        
        if (MaintainAspectRatio)
        {
            matrixStack.Scale(Texture.Width / (double) Texture.Height, 1.0, 1.0);
        }
        
        matrixStack.Scale(Scale);
        matrixStack.Rotate(Rotation);
        matrixStack.Translate(Position);

        vertexDrawData.Transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        vertexDrawData.Texture = Texture;
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
        
        matrixStack.Pop();
    }
    
    public override void Dispose()
    {
    }
}