using System.Collections;

namespace FlexFramework.Util;

public class Node<T> : IEnumerable<Node<T>>
{
    public T Value { get; }
    public List<Node<T>> Children { get; }

    public Node(T value, IEnumerable<Node<T>> children)
    {
        Value = value;
        Children = children.ToList();
    }

    public Node(T value) : this(value, Enumerable.Empty<Node<T>>())
    {
    }

    public IEnumerator<Node<T>> GetEnumerator()
    {
        Stack<Node<T>> stack = new Stack<Node<T>>();
        stack.Push(this);

        while (stack.Count > 0)
        {
            Node<T> node = stack.Pop();
            yield return node;

            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(node.Children[i]);
            }
        }
    }
    
    public Node<TResult> Transform<TResult>(Func<T, TResult> transformer)
    {
        return new Node<TResult>(transformer(Value), Children.Select(child => child.Transform(transformer)));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}