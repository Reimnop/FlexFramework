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
    public Font Font
    {
        get => font;
        set
        {
            font = value;
            InvalidateMesh();
        }
    }

    public int BaselineOffset
    {
        get => baselineOffset;
        set
        {
            baselineOffset = value;
            InvalidateMesh();
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => horizontalAlignment;
        set
        {
            horizontalAlignment = value;
            InvalidateMesh();
        }
    }

    public VerticalAlignment VerticalAlignment
    {
        get => verticalAlignment;
        set
        {
            verticalAlignment = value;
            InvalidateMesh();
        }
    }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            InvalidateMesh();
        }
    }
    
    public Color4 Color { get; set; } = Color4.White;

    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
    private int baselineOffset = 0;
    private Font font = null;
    private string text = "";

    private bool meshValid = false;

    private readonly FlexFrameworkMain engine;
    private readonly TextDrawData textDrawData;
    private readonly Mesh<TextVertex> mesh;

    public TextEntity(FlexFrameworkMain engine, Font font)
    {
        this.engine = engine;
        this.font = font;
        HorizontalAlignment = horizontalAlignment;
        
        mesh = new Mesh<TextVertex>("text");
        mesh.Attribute(2, 0, VertexAttribType.Float, false);
        mesh.Attribute(4, 2 * sizeof(float), VertexAttribType.Float, false);
        mesh.Attribute(2, 6 * sizeof(float), VertexAttribType.Float, false);
        mesh.Attribute(1, 8 * sizeof(float), VertexAttribIntegerType.Int);
        
        textDrawData = new TextDrawData(mesh.VertexArray, mesh.Count, Matrix4.Identity, Color);
    }

    public void InvalidateMesh()
    {
        meshValid = false;
    }

    private void GenerateMesh()
    {
        TextBuilder builder = new TextBuilder(engine.TextResources.Fonts)
            .WithBaselineOffset(baselineOffset)
            .WithHorizontalAlignment(horizontalAlignment)
            .WithVerticalAlignment(verticalAlignment)
            .AddText(new StyledText(text, font)
                .WithColor(System.Drawing.Color.White));
        
        TextVertex[] vertices = TextMeshGenerator.GenerateVertices(builder.Build());
        mesh.LoadData(vertices);
    }

    public override void Update(UpdateArgs args)
    {
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (!meshValid)
        {
            meshValid = true;
            GenerateMesh();
        }
        
        if (mesh.Count == 0)
        {
            return;
        }

        textDrawData.Count = mesh.Count;
        textDrawData.Transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        textDrawData.Color = Color;
        
        renderer.EnqueueDrawData(layerId, textDrawData);
    }
    
    public override void Dispose()
    {
        mesh.Dispose();
    }
}