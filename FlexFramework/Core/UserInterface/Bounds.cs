using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public struct Bounds
{
    public float Width
    {
        get => X1 - X0;
        set => X1 = X0 + value;
    }

    public float Height
    {
        get => Y1 - Y0;
        set => Y1 = Y0 + value;
    }

    public Vector2 Min
    {
        get => new Vector2(X0, Y0);
        set
        {
            X0 = value.X;
            Y0 = value.Y;
        }
    }
    
    public Vector2 Max
    {
        get => new Vector2(X1, Y1);
        set
        {
            X1 = value.X;
            Y1 = value.Y;
        }
    }
    
    public float X0 { get; set; }
    public float Y0 { get; set; }
    public float X1 { get; set; }
    public float Y1 { get; set; }
    
    public Bounds(float x0, float y0, float x1, float y1)
    {
        X0 = x0;
        Y0 = y0;
        X1 = x1;
        Y1 = y1;
    }

    public bool Contains(float x, float y)
    {
        return x > X0 && x < X1 && y > Y0 && y < Y1;
    }
    
    public bool Contains(Vector2 point)
    {
        return Contains(point.X, point.Y);
    }
    
    public static Bounds operator +(Bounds a, Bounds b)
    {
        return new Bounds(a.X0 + b.X0, a.Y0 + b.Y0, a.X1 + b.X1, a.Y1 + b.Y1);
    }
    
    public static Bounds operator -(Bounds a, Bounds b)
    {
        return new Bounds(a.X0 - b.X0, a.Y0 - b.Y0, a.X1 - b.X1, a.Y1 - b.Y1);
    }
    
    public static Bounds operator *(Bounds a, float b)
    {
        return new Bounds(a.X0 * b, a.Y0 * b, a.X1 * b, a.Y1 * b);
    }
    
    public static Bounds operator *(float a, Bounds b)
    {
        return new Bounds(a * b.X0, a * b.Y0, a * b.X1, a * b.Y1);
    }
    
    public static Bounds operator /(Bounds a, float b)
    {
        return new Bounds(a.X0 / b, a.Y0 / b, a.X1 / b, a.Y1 / b);
    }
    
    public static Bounds operator /(float a, Bounds b)
    {
        return new Bounds(a / b.X0, a / b.Y0, a / b.X1, a / b.Y1);
    }
    
    public static bool operator ==(Bounds a, Bounds b)
    {
        return a.X0 == b.X0 && a.Y0 == b.Y0 && a.X1 == b.X1 && a.Y1 == b.Y1;
    }
    
    public static bool operator !=(Bounds a, Bounds b)
    {
        return a.X0 != b.X0 || a.Y0 != b.Y0 || a.X1 != b.X1 || a.Y1 != b.Y1;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Bounds bounds && this == bounds;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(X0, Y0, X1, Y1);
    }
    
    public override string ToString()
    {
        return $"({X0}, {Y0}) - ({X1}, {Y1})";
    }
    
    public static implicit operator Bounds((float x0, float y0, float x1, float y1) value)
    {
        return new Bounds(value.x0, value.y0, value.x1, value.y1);
    }
}