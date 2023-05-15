using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    [VertexAttribute(VertexAttributeIntent.Position, VertexAttributeType.Float, 3)]
    public Vector3 Position;

    [VertexAttribute(VertexAttributeIntent.TexCoord0, VertexAttributeType.Float, 2)]
    public Vector2 Uv;

    [VertexAttribute(VertexAttributeIntent.Color, VertexAttributeType.Float, 4)]
    public Color4 Color;

    public Vertex(Vector3 position, Vector2 uv)
    {
        Position = position;
        Uv = uv;
        Color = Color4.White;
    }
    
    public Vertex(float x, float y, float z, float u, float v)
    {
        Position = new Vector3(x, y, z);
        Uv = new Vector2(u, v);
        Color = Color4.White;
    }
    
    public Vertex(Vector3 position, Vector2 uv, Color4 color)
    {
        Position = position;
        Uv = uv;
        Color = color;
    }
    
    public Vertex(float x, float y, float z, float u, float v, float r, float g, float b, float a)
    {
        Position = new Vector3(x, y, z);
        Uv = new Vector2(u, v);
        Color = new Color4(r, g, b, a);
    }
}