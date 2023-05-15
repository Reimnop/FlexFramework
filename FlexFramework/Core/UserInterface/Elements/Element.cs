using System.Collections;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element : IEnumerable<Element>
{
#if DEBUG_SHOW_BOUNDING_BOXES // This will blow up if you have multiple OpenGL contexts, but it's only for debugging anyway
    private static readonly Mesh<Vertex> DebugMesh = new Mesh<Vertex>("debug");

    static Element()
    {
        Vertex[] debugVertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        DebugMesh.SetData(debugVertices);
    }
#endif
    
    public List<Element> Children { get; } = new List<Element>();

    public Length Width { get; set; } = Length.Zero;
    public Length Height { get; set; } = Length.Zero;

    public Length Margin
    {
        set => MarginLeft = MarginRight = MarginTop = MarginBottom = value;
    }
    public Length MarginLeft { get; set; } = Length.Zero;
    public Length MarginRight { get; set; } = Length.Zero;
    public Length MarginTop { get; set; } = Length.Zero;
    public Length MarginBottom { get; set; } = Length.Zero;
    
    public Length Padding
    {
        set => PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = value;
    }
    public Length PaddingLeft { get; set; } = Length.Zero;
    public Length PaddingRight { get; set; } = Length.Zero;
    public Length PaddingTop { get; set; } = Length.Zero;
    public Length PaddingBottom { get; set; } = Length.Zero;
    
    protected Bounds BoundingBox { get; private set; }
    protected Bounds ElementBounds { get; private set; }
    protected Bounds ContentBounds { get; private set; }
    
    protected Element(params Element[] children)
    {
        Children.AddRange(children);
    }

    // Bounding box is the area that the element occupies
    public Bounds CalculateBoundingBox(Bounds constraintBounds)
    {
        return new Bounds(
            constraintBounds.X0, 
            constraintBounds.Y0, 
            constraintBounds.X0 + Width.Calculate(constraintBounds.Width), 
            constraintBounds.Y0 + Height.Calculate(constraintBounds.Height));
    }

    // Element bounds is the area that the element can draw to
    public Bounds CalculateElementBounds(Bounds boundingBox)
    {
        return new Bounds(
            boundingBox.X0 + MarginLeft.Calculate(boundingBox.Width),
            boundingBox.Y0 + MarginTop.Calculate(boundingBox.Height),
            boundingBox.X1 - MarginRight.Calculate(boundingBox.Width),
            boundingBox.Y1 - MarginBottom.Calculate(boundingBox.Height));
    }
    
    // Content bounds is the area where the element's children can draw to
    public Bounds CalculateContentBounds(Bounds elementBounds)
    {
        return new Bounds(
            elementBounds.X0 + PaddingLeft.Calculate(elementBounds.Width),
            elementBounds.Y0 + PaddingTop.Calculate(elementBounds.Height),
            elementBounds.X1 - PaddingRight.Calculate(elementBounds.Width),
            elementBounds.Y1 - PaddingBottom.Calculate(elementBounds.Height));
    }
    
    public void CalculateBounds(Bounds constraintBounds, out Bounds boundingBox, out Bounds elementBounds, out Bounds contentBounds)
    {
        boundingBox = CalculateBoundingBox(constraintBounds);
        elementBounds = CalculateElementBounds(boundingBox);
        contentBounds = CalculateContentBounds(elementBounds);
    }

    public virtual void UpdateLayout(Bounds constraintBounds)
    {
        CalculateBounds(constraintBounds, out Bounds boundingBox, out Bounds elementBounds, out Bounds contentBounds);
        BoundingBox = boundingBox;
        ElementBounds = elementBounds;
        ContentBounds = contentBounds;
    }
    
    protected void UpdateChildrenLayout(Bounds contentBounds)
    {
        float y = contentBounds.Y0;
        
        // Render children
        foreach (Element child in Children)
        {
            Bounds childConstraintBounds = new Bounds(contentBounds.X0, y, contentBounds.X1, contentBounds.Y1);
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height;

            child.UpdateLayout(childConstraintBounds);
        }
    }

    /// <summary>
    /// Enumerates all elements of the tree, including this element.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Element> GetEnumerator()
    {
        Stack<Element> stack = new Stack<Element>();
        stack.Push(this);
        
        while (stack.Count > 0)
        {
            Element element = stack.Pop();
            yield return element;
            
            int childCount = element.Children.Count;
            for (int i = childCount - 1; i >= 0; i--)
            {
                stack.Push(element.Children[i]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    // Helper methods
    public void UpdateRecursive(UpdateArgs args)
    {
        foreach (IUpdateable updateable in this.OfType<IUpdateable>())
        {
            updateable.Update(args);
        }
    }

    public void RenderRecursive(RenderArgs args)
    {
        foreach (IRenderable renderable in this.OfType<IRenderable>())
        {
            renderable.Render(args);
        }
        
#if DEBUG_SHOW_BOUNDING_BOXES
        foreach (Element element in this)
        {
            DrawDebugBoxes(element, args);
        }
#endif
    }
    
    public void DisposeRecursive()
    {
        foreach (IDisposable disposable in this.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
    
#if DEBUG_SHOW_BOUNDING_BOXES
    protected static void DrawDebugBoxes(Element element, RenderArgs args)
    {
        RenderBounds(element.BoundingBox, Color4.Red, args);
        RenderBounds(element.ElementBounds, Color4.Green, args);
        RenderBounds(element.ContentBounds, Color4.Blue, args);
    }

    private static void RenderBounds(Bounds bounds, Color4 color, RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        Vector2 size = bounds.Max - bounds.Min;
        
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(size.X, size.Y, 1.0f);
        matrixStack.Translate(bounds.X0, bounds.Y0, 0.0f);
        VertexDrawData vertexDrawData = new VertexDrawData(DebugMesh.VertexArray, DebugMesh.Count, matrixStack.GlobalTransformation * args.CameraData.View * args.CameraData.Projection, null, color, PrimitiveType.LineLoop);
        args.Renderer.EnqueueDrawData(args.LayerId, vertexDrawData);
        matrixStack.Pop();
    }
#endif
}