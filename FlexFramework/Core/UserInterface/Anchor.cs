using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public struct Anchor
{
    public static Anchor TopLeft => new(new Vector2(0.0f, 0.0f));
    public static Anchor TopCenter => new(new Vector2(0.5f, 0.0f));
    public static Anchor TopRight => new(new Vector2(1.0f, 0.0f));
    public static Anchor CenterLeft => new(new Vector2(0.0f, 0.5f));
    public static Anchor Center => new(new Vector2(0.5f, 0.5f));
    public static Anchor CenterRight => new(new Vector2(1.0f, 0.5f));
    public static Anchor BottomLeft => new(new Vector2(0.0f, 1.0f));
    public static Anchor BottomCenter => new(new Vector2(0.5f, 1.0f));
    public static Anchor BottomRight => new(new Vector2(1.0f, 1.0f));
    public static Anchor Fill => new(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f));
    public static Anchor FillLeftEdge => new(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 1.0f));
    public static Anchor FillRightEdge => new(new Vector2(1.0f, 0.0f), new Vector2(1.0f, 1.0f));
    public static Anchor FillTopEdge => new(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f));
    public static Anchor FillBottomEdge => new(new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f));
    
    public Vector2 Min { get; set; }
    public Vector2 Max { get; set; }
    
    public Anchor(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }
    
    public Anchor(float minX, float minY, float maxX, float maxY) : this(new Vector2(minX, minY), new Vector2(maxX, maxY))
    {
    }

    public Anchor(Vector2 position) : this(position, position)
    {
    }

    public Box2 GetBounds(Box2 parentBounds, Edges edges)
    {
        var min = parentBounds.Min + Min * parentBounds.Size;
        var max = parentBounds.Min + Max * parentBounds.Size;
        min.X += edges.Left;
        min.Y += edges.Top;
        max.X -= edges.Right;
        max.Y -= edges.Bottom;
        
        return new Box2(min, max);
    }
}