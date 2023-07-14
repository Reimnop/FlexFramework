using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

/// <summary>
/// Represents the edge dimensions.
/// </summary>
public struct Edges
{
    public float Top { get; set; }
    public float Bottom { get; set; }
    public float Left { get; set; }
    public float Right { get; set; }
    
    public Edges(float top, float bottom, float left, float right)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
    }

    public Edges(float value) : this(value, value, value, value)
    {
    }
    
    public Edges WithTop(float value)
    {
        return new Edges(value, Bottom, Left, Right);
    }
    
    public Edges WithBottom(float value)
    {
        return new Edges(Top, value, Left, Right);
    }
    
    public Edges WithLeft(float value)
    {
        return new Edges(Top, Bottom, value, Right);
    }
    
    public Edges WithRight(float value)
    {
        return new Edges(Top, Bottom, Left, value);
    }
    
    public Edges Translate(Vector2 value)
    {
        return new Edges(Top + value.Y, Bottom - value.Y, Left + value.X, Right - value.X);
    }
    
    public Edges Translate(float x, float y)
    {
        return new Edges(Top + y, Bottom - y, Left + x, Right - x);
    }
}