using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public static class LayoutEngine
{
    public static void Layout(Node<ElementContainer> root, Box2 bounds, float dpiScale = 1.0f)
    {
        LayoutRecursively(root, bounds, dpiScale);
    }

    private static void LayoutRecursively(Node<ElementContainer> node, Box2 parentBounds, float dpiScale)
    {
        var elementContainer = node.Value;
        var bounds = elementContainer.Anchor.GetBounds(parentBounds, elementContainer.Edges);
        node.Value.Element.SetLayout(bounds, dpiScale);
        
        foreach (var child in node.Children)
        {
            LayoutRecursively(child, bounds, dpiScale);
        }
    }
}