namespace FlexFramework.Util;

public class TreeBuilder<T>
{
    private T value;
    private List<TreeBuilder<T>> children = new List<TreeBuilder<T>>();

    public TreeBuilder(T value)
    {
        this.value = value;
    }

    public TreeBuilder<T> PushChild(TreeBuilder<T> childBuilder)
    {
        children.Add(childBuilder);
        return this;
    }

    public TreeBuilder<T> PushChild(T value, Action<TreeBuilder<T>>? childBuilderConsumer = null)
    {
        TreeBuilder<T> childBuilder = new TreeBuilder<T>(value);
        childBuilderConsumer?.Invoke(childBuilder);
        return PushChild(childBuilder);
    }

    public Node<T> Build()
    {
        return new Node<T>(value, BuildChildren(this));
    }

    private static IEnumerable<Node<T>> BuildChildren(TreeBuilder<T> parent)
    {
        foreach (var childBuilder in parent.children)
        {
            yield return new Node<T>(childBuilder.value, BuildChildren(childBuilder));
        }
    }
}