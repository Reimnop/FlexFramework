using FlexFramework.Util;

namespace FlexFramework.Core.UserInterface;

public static class Extensions
{
    public static void UpdateRecursively(this Node<ElementContainer> node, UpdateArgs args)
    {
        foreach (var updatable in node.Select(x => x.Value.Element).OfType<IUpdateable>())
        {
            updatable.Update(args);
        }
    }
    
    public static void RenderRecursively(this Node<ElementContainer> node, RenderArgs args)
    {
        foreach (var renderable in node.Select(x => x.Value.Element).OfType<IRenderable>())
        {
            renderable.Render(args);
        }
    }
    
    public static void DisposeRecursively(this Node<ElementContainer> node)
    {
        foreach (var disposable in node.Select(x => x.Value.Element).OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}