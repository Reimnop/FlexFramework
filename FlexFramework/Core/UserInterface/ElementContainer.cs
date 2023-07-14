using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public class ElementContainer
{
    public Element Element { get; }
    public Box2 Bounds => Element.Bounds;
    public Edges Edges { get; set; }
    public Anchor Anchor { get; set; } = Anchor.TopLeft;

    public ElementContainer(Element element)
    {
        Element = element;
    }

    public ElementContainer SetEdges(Edges edges)
    {
        Edges = edges;
        return this;
    }
    
    public ElementContainer SetEdges(float top, float bottom, float left, float right)
    {
        return SetEdges(new Edges(top, bottom, left, right));
    }
    
    public ElementContainer SetEdges(float value)
    {
        return SetEdges(new Edges(value));
    }
    
    public ElementContainer SetAnchor(Anchor anchor)
    {
        Anchor = anchor;
        return this;
    }
    
    public ElementContainer SetAnchor(Vector2 min, Vector2 max)
    {
        return SetAnchor(new Anchor(min, max));
    }
    
    public ElementContainer SetAnchor(float minX, float minY, float maxX, float maxY)
    {
        return SetAnchor(new Anchor(minX, minY, maxX, maxY));
    }
    
    public ElementContainer SetAnchor(Vector2 position)
    {
        return SetAnchor(new Anchor(position));
    }
}