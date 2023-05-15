using System.Collections;

namespace FlexFramework.Modelling;

public class ImmutableNode<T> : IEnumerable<ImmutableNode<T>>
{
    public T Value { get; }
    public IReadOnlyList<ImmutableNode<T>> Children => children;

    private readonly List<ImmutableNode<T>> children;

    public ImmutableNode(T value, IEnumerable<ImmutableNode<T>> children)
    {
        Value = value;
        this.children = children.ToList();
    }

    public ImmutableNode(T value) : this(value, Enumerable.Empty<ImmutableNode<T>>())
    {
    }

    public IEnumerator<ImmutableNode<T>> GetEnumerator()
    {
        Stack<ImmutableNode<T>> stack = new Stack<ImmutableNode<T>>();
        stack.Push(this);

        while (stack.Count > 0)
        {
            ImmutableNode<T> node = stack.Pop();
            yield return node;

            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(node.Children[i]);
            }
        }
    }
    
    public ImmutableNode<TResult> Transform<TResult>(Func<T, TResult> transformer)
    {
        return new ImmutableNode<TResult>(transformer(Value), Children.Select(child => child.Transform(transformer)));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}