using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public class InterfaceTreeBuilder
{
    private ElementContainer element = new(new EmptyElement());
    private readonly List<InterfaceTreeBuilder> children = new();

    public InterfaceTreeBuilder SetElement(Element element)
    {
        this.element = new ElementContainer(element);
        return this;
    }
    
    public InterfaceTreeBuilder AddChild(InterfaceTreeBuilder child)
    {
        children.Add(child);
        return this;
    }

    public InterfaceTreeBuilder SetEdges(Edges edges)
    {
        element.SetEdges(edges);
        return this;
    }
    
    public InterfaceTreeBuilder SetEdges(float top, float bottom, float left, float right)
    {
        element.SetEdges(top, bottom, left, right);
        return this;
    }
    
    public InterfaceTreeBuilder SetEdges(float value)
    {
        element.SetEdges(value);
        return this;
    }
    
    public InterfaceTreeBuilder SetAnchor(Anchor anchor)
    {
        element.SetAnchor(anchor);
        return this;
    }
    
    public InterfaceTreeBuilder SetAnchor(Vector2 min, Vector2 max)
    {
        element.SetAnchor(min, max);
        return this;
    }
    
    public InterfaceTreeBuilder SetAnchor(float minX, float minY, float maxX, float maxY)
    {
        element.SetAnchor(minX, minY, maxX, maxY);
        return this;
    }

    public InterfaceTreeBuilder SetAnchor(Vector2 position)
    {
        element.SetAnchor(position);
        return this;
    }

    public Node<ElementContainer> Build()
    {
        return new Node<ElementContainer>(element, children.Select(x => x.Build()));
    }
}