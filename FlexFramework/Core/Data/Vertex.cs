using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector3 Position { get; set; }
    public Vector2 Uv { get; set; }

    public Vertex(Vector3 position, Vector2 uv)
    {
        Position = position;
        Uv = uv;
    }
    
    public Vertex(float x, float y, float z, float u, float v)
    {
        Position = new Vector3(x, y, z);
        Uv = new Vector2(u, v);
    }
}