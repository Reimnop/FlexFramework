using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Text;
using OpenTK.Mathematics;
using Buffer = FlexFramework.Core.Data.Buffer;

namespace FlexFramework.Core.Entities;

public class TextEntity : Entity, IRenderable
{
    public int BaselineOffset { get; set; }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            InvalidateTextData();
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => horizontalAlignment;
        set
        {
            horizontalAlignment = value;
            InvalidateTextData();
        }
    }

    public VerticalAlignment VerticalAlignment
    {
        get => verticalAlignment;
        set
        {
            verticalAlignment = value;
            InvalidateTextData();
        }
    }

    public Box2 Bounds
    {
        get => bounds;
        set
        {
            bounds = value;
            InvalidateTextData();
        }
    }

    public Font Font
    {
        get => font;
        set
        {
            font = value;
            InvalidateTextData();
        }
    }

    public float EmSize
    {
        get => emSize;
        set
        {
            emSize = value;
            InvalidateTextData();
        }
    }

    public float DpiScale { get; set; } = 1.0f;

    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Top;
    private Box2 bounds = new(0.0f, 0.0f, 0.0f, 0.0f);
    private float emSize = 1.0f;

    public Color4 Color { get; set; } = Color4.White;
    
    private Font font;
    private string text = "";

    private bool dataValid = false;
    
    private readonly Mesh<TextVertex> mesh;
    private readonly Texture fontAtlas;
    
    private readonly Buffer buffer = new();

    public TextEntity(Font font)
    {
        this.font = font;
        fontAtlas = new Texture("font_atlas", font.Texture.Width, font.Texture.Height, PixelFormat.Rgb32f);
        fontAtlas.SetData<Rgb32f>(font.Texture.Pixels);

        var vertexLayout = new VertexLayout(
            Unsafe.SizeOf<TextVertex>(),
            new VertexAttribute(VertexAttributeIntent.Position, VertexAttributeType.Float, 2, 0),
            new VertexAttribute(VertexAttributeIntent.TexCoord0, VertexAttributeType.Float, 2, 2 * sizeof(float))
        );

        mesh = new Mesh<TextVertex>("text", vertexLayout);
    }

    private void InvalidateTextData()
    {
        dataValid = false;
    }

    private void UpdateTextData()
    {
        var boundsMinX = (int) (bounds.Min.X * 64.0f);
        var boundsMinY = (int) (bounds.Min.Y * 64.0f);
        var boundsMaxX = (int) (bounds.Max.X * 64.0f);
        var boundsMaxY = (int) (bounds.Max.Y * 64.0f);
        var shapedText = TextShaper.ShapeText(font, text, boundsMinX, boundsMinY, boundsMaxX, boundsMaxY, emSize, horizontalAlignment, verticalAlignment);
        
        buffer.Clear();
        MeshGenerator.GenerateMesh(v => buffer.Append(v), shapedText);
        
        // Convert ReadOnlySpan<byte> to ReadOnlySpan<TextVertex>
        var textVertices = MemoryMarshal.Cast<byte, TextVertex>(buffer.Data);
        mesh.SetData(textVertices, null);
    }

    public void Render(RenderArgs args)
    {
        if (!dataValid)
        {
            dataValid = true;
            UpdateTextData();
        }

        CommandList commandList = args.CommandList;
        LayerType layerType = args.LayerType;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;
        
        matrixStack.Push();
        matrixStack.Translate(0.0f, BaselineOffset / 64.0f, 0.0f);

        var transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        var textDrawData = new TextDrawData(mesh.ReadOnly, fontAtlas.ReadOnly, transformation, Color, 4.0f * EmSize * DpiScale);
        
        commandList.AddDrawData(layerType, textDrawData);
        matrixStack.Pop();
    }
}