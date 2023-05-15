using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public struct BoneWeight
{
    public int Index { get; }
    public float Weight { get; }

    public BoneWeight(int index, float weight)
    {
        Index = index;
        Weight = weight;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SkinnedVertex
{
    public const int MaxBoneInfluence = 4;

    [VertexAttribute(VertexAttributeIntent.Position, VertexAttributeType.Float, 3)]
    public Vector3 Position;

    [VertexAttribute(VertexAttributeIntent.Normal, VertexAttributeType.Float, 3)]
    public Vector3 Normal;

    [VertexAttribute(VertexAttributeIntent.TexCoord0, VertexAttributeType.Float, 2)]
    public Vector2 Uv;

    [VertexAttribute(VertexAttributeIntent.Color, VertexAttributeType.Float, 4)]
    public Color4 Color;

    [VertexAttribute(VertexAttributeIntent.BoneIndex, VertexAttributeType.Int, MaxBoneInfluence)]
    public fixed int BoneIndices[MaxBoneInfluence];
    
    [VertexAttribute(VertexAttributeIntent.BoneWeight, VertexAttributeType.Float, MaxBoneInfluence)]
    public fixed float Weights[MaxBoneInfluence];

    public SkinnedVertex(Vector3 position, Vector3 normal, Vector2 uv, Color4 color, params BoneWeight[] weights)
    {
        Position = position;
        Normal = normal;
        Uv = uv;
        Color = color;

        if (weights.Length > MaxBoneInfluence)
        {
            throw new IndexOutOfRangeException();
        }

        for (int i = 0; i < weights.Length; i++)
        {
            BoneIndices[i] = weights[i].Index;
            Weights[i] = weights[i].Weight;
        }
        
        for (int i = weights.Length; i < MaxBoneInfluence; i++)
        {
            BoneIndices[i] = -1;
            Weights[i] = 0.0f;
        }
    }
    
    public SkinnedVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v, float r, float g, float b, float a, params BoneWeight[] weights)
    {
        Position = new Vector3(x, y, z);
        Normal = new Vector3(nx, ny, nz);
        Uv = new Vector2(u, v);
        Color = new Color4(r, g, b, a);
        
        if (weights.Length > MaxBoneInfluence)
        {
            throw new IndexOutOfRangeException();
        }

        for (int i = 0; i < weights.Length; i++)
        {
            BoneIndices[i] = weights[i].Index;
            Weights[i] = weights[i].Weight;
        }

        for (int i = weights.Length; i < MaxBoneInfluence; i++)
        {
            BoneIndices[i] = -1;
            Weights[i] = 0.0f;
        }
    }

    public SkinnedVertex AppendWeight(BoneWeight weight)
    {
        SkinnedVertex newVertex = this;
        
        for (int i = 0; i < MaxBoneInfluence; i++)
        {
            if (newVertex.BoneIndices[i] == -1)
            {
                newVertex.BoneIndices[i] = weight.Index;
                newVertex.Weights[i] = weight.Weight;
                return newVertex;
            }
        }

        return newVertex;
    }
}