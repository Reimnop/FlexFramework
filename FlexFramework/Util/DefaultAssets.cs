using FlexFramework.Core.Data;

namespace FlexFramework.Util;

public static class DefaultAssets
{
    public static Mesh<Vertex> QuadMesh { get; } = CreateQuadMesh();
    public static Mesh<Vertex> QuadWireframeMesh { get; } = CreateQuadWireframeMesh();

    private static Mesh<Vertex> CreateQuadWireframeMesh()
    {
        Vertex[] vertices =
        {
            new(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        return new Mesh<Vertex>("quad_wireframe", vertices).SetReadOnly();
    }
    
    private static Mesh<Vertex> CreateQuadMesh()
    {
        Vertex[] vertices =
        {
            new(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        return new Mesh<Vertex>("quad", vertices).SetReadOnly();
    }
}