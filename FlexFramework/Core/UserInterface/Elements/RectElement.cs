using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : VisualElement, IRenderable
{
    public Color4 Color
    {
        get => rectEntity.Color;
        set => rectEntity.Color = value;
    }
    
    public float Radius
    {
        get => rectEntity.Radius;
        set => rectEntity.Radius = value;
    }
    
    public float BorderThickness
    {
        get => rectEntity.BorderThickness;
        set => rectEntity.BorderThickness = value;
    }

    private readonly RectEntity rectEntity = new();

    protected override void UpdateLayout(Box2 bounds)
    {
        rectEntity.Bounds = bounds;
    }

    public override void Render(RenderArgs args)
    {
        rectEntity.Render(args);
    }
}