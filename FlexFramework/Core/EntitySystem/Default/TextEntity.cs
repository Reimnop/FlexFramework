using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.EntitySystem.Default;

public class TextEntity : Entity, IRenderable
{
    private TextDrawData textDrawData;

    private Mesh<TextVertex> mesh;

    public TextEntity()
    {
        mesh = new Mesh<TextVertex>("text");
        mesh.Attribute(2, 0, VertexAttribType.Float, false);
        mesh.Attribute(4, 2 * sizeof(float), VertexAttribType.Float, false);
        mesh.Attribute(2, 6 * sizeof(float), VertexAttribType.Float, false);
        mesh.Attribute(1, 8 * sizeof(float), VertexAttribIntegerType.Int);
        
        textDrawData = new TextDrawData(mesh.VertexArray, mesh.Count, Matrix4.Identity);
    }

    public void SetText(BuiltText text)
    {
        TextVertex[] vertices = TextMeshGenerator.GenerateVertices(text);
        mesh.LoadData(vertices);
    }
    
    public override void Update(UpdateArgs args)
    {
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (mesh.Count == 0)
        {
            return;
        }
        
        textDrawData.Count = mesh.Count;
        textDrawData.Transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        
        renderer.EnqueueDrawData(layerId, textDrawData);
    }
    
    public override void Dispose()
    {
        mesh.Dispose();
    }
}