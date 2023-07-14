using System.Runtime.InteropServices;

namespace FlexFramework.Text;

/// <summary>
/// Represents a color with red, green and blue components.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rgb32f
{
    public float R;
    public float G;
    public float B;

    public Rgb32f(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }
}