using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public struct LitVertex
{
    [VertexAttribute(VertexAttributeIntent.Position, VertexAttributeType.Float, 3)]
    public Vector3 Position;

    [VertexAttribute(VertexAttributeIntent.Normal, VertexAttributeType.Float, 3)]
    public Vector3 Normal;

    [VertexAttribute(VertexAttributeIntent.TexCoord0, VertexAttributeType.Float, 2)]
    public Vector2 Uv;

    [VertexAttribute(VertexAttributeIntent.Color, VertexAttributeType.Float, 4)]
    public Color4 Color;

    public LitVertex(Vector3 position, Vector3 normal, Vector2 uv, Color4 color)
    {
        Position = position;
        Normal = normal;
        Uv = uv;
        Color = color;
    }
    
    public LitVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v, float r, float g, float b, float a)
    {
        Position = new Vector3(x, y, z);
        Normal = new Vector3(nx, ny, nz);
        Uv = new Vector2(u, v);
        Color = new Color4(r, g, b, a);
    }
}