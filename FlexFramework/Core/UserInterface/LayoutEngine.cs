using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public static class LayoutEngine
{
    public static void Layout(Node<ElementContainer> root, Box2 bounds)
    {
        LayoutRecursively(root, bounds);
    }

    private static void LayoutRecursively(Node<ElementContainer> node, Box2 parentBounds)
    {
        var elementContainer = node.Value;
        var bounds = elementContainer.Anchor.GetBounds(parentBounds, elementContainer.Edges);
        node.Value.Element.SetBounds(bounds);
        
        foreach (var child in node.Children)
        {
            LayoutRecursively(child, bounds);
        }
    }
}