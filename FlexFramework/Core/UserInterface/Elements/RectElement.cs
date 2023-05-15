using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : VisualElement, IRenderable
{
    public float Radius
    {
        get => rectEntity.Radius;
        set => rectEntity.Radius = value;
    }

    public Color4 Color
    {
        get => rectEntity.Color;
        set => rectEntity.Color = value;
    }

    private readonly RectEntity rectEntity = new RectEntity();

    public RectElement(params Element[] children) : base(children)
    {
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        rectEntity.Min = ElementBounds.Min;
        rectEntity.Max = ElementBounds.Max;
    }

    public override void Render(RenderArgs args)
    {
        rectEntity.Render(args);
    }
}