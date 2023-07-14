using FlexFramework.Core.Entities;
using FlexFramework.Text;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : VisualElement, IRenderable
{
    public string Text
    {
        get => textEntity.Text;
        set => textEntity.Text = value;
    }
    
    public float EmSize
    {
        get => textEntity.EmSize;
        set => textEntity.EmSize = value;
    }
    
    public HorizontalAlignment HorizontalAlignment
    {
        get => textEntity.HorizontalAlignment;
        set => textEntity.HorizontalAlignment = value;
    }
    
    public VerticalAlignment VerticalAlignment
    {
        get => textEntity.VerticalAlignment;
        set => textEntity.VerticalAlignment = value;
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }
    
    private readonly TextEntity textEntity;

    public TextElement(Font font)
    {
        textEntity = new TextEntity(font);
    }

    protected override void UpdateLayout(Box2 bounds, float dpiScale)
    {
        textEntity.Bounds = bounds;
        textEntity.DpiScale = dpiScale;
    }

    public override void Render(RenderArgs args)
    {
        textEntity.Render(args);
    }
}