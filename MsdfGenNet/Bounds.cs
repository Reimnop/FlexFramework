using System.Runtime.InteropServices;

namespace MsdfGenNet;

[StructLayout(LayoutKind.Sequential)]
public struct Bounds
{
    public double Left
    {
        get => l;
        set => l = value;
    }
    
    public double Bottom
    {
        get => b;
        set => b = value;
    }
    
    public double Right
    {
        get => r;
        set => r = value;
    }
    
    public double Top
    {
        get => t;
        set => t = value;
    }
    
    private double l, b, r, t;
    
    public Bounds(double left, double bottom, double right, double top)
    {
        l = left;
        b = bottom;
        r = right;
        t = top;
    }
}