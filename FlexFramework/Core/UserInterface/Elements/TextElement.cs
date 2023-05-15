using FlexFramework.Core.Entities;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : VisualElement, IRenderable
{
    public string Text
    {
        get => textEntity.Text;
        set
        {
            textEntity.Text = value;

            if (autoHeight)
            {
                int lines = value.Split('\n').Length;
                float height = lines * (font.Height / 64) * textEntity.EmSize;
                Height = height;
            }
        }
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }

    private readonly TextEntity textEntity;
    private readonly Font font;
    private readonly bool autoHeight;

    public TextElement(FlexFrameworkMain engine, string fontName, bool autoHeight = true, params Element[] children) : base(children)
    {
        this.autoHeight = autoHeight;
        
        var textAssetsLocation = engine.DefaultAssets.TextAssets;
        var textAssets = engine.ResourceRegistry.GetResource(textAssetsLocation);
        font = textAssets[fontName];
        
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
    }
    
    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(ElementBounds.X0, ElementBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }
}