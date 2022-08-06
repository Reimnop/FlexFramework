using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public class MeshEntity : Entity, IRenderable
{
    public Mesh<Vertex>? Mesh { get; set; }

    private readonly VertexDrawData vertexDrawData;
    
    public MeshEntity()
    {
        vertexDrawData = new VertexDrawData(null, 0, Matrix4.Identity);
    }
    
    public override void Update(UpdateArgs args)
    {
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Mesh == null)
        {
            return;
        }

        vertexDrawData.VertexArray = Mesh.VertexArray;
        vertexDrawData.Count = Mesh.Count;

        vertexDrawData.Transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
    
    public override void Dispose()
    {
    }
}