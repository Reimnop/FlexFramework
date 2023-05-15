using System.Numerics;
using System.Runtime.InteropServices;

namespace MsdfGenNet;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2d
{
    public double X { get; set; }
    public double Y { get; set; }

    public Vector2d(double v)
    {
        X = v;
        Y = v;
    }

    public Vector2d(double x, double y)
    {
        X = x;
        Y = y;
    }
    
    // Conversion operators
    public static implicit operator Vector2d(Vector2 v) => new Vector2d(v.X, v.Y);
    public static explicit operator Vector2(Vector2d v) => new Vector2((float) v.X, (float) v.Y); // This is explicit because it can lose precision
    
    // Arithmetic operators
    public static Vector2d operator +(Vector2d a, Vector2d b) => new Vector2d(a.X + b.X, a.Y + b.Y);
    public static Vector2d operator -(Vector2d a, Vector2d b) => new Vector2d(a.X - b.X, a.Y - b.Y);
    public static Vector2d operator *(Vector2d a, double b) => new Vector2d(a.X * b, a.Y * b);
    public static Vector2d operator *(double a, Vector2d b) => new Vector2d(a * b.X, a * b.Y);
    public static Vector2d operator /(Vector2d a, double b) => new Vector2d(a.X / b, a.Y / b);
    
    // Unary operators
    public static Vector2d operator -(Vector2d v) => new Vector2d(-v.X, -v.Y);
    
    // Equality operators
    public static bool operator ==(Vector2d a, Vector2d b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2d a, Vector2d b) => a.X != b.X || a.Y != b.Y;
}