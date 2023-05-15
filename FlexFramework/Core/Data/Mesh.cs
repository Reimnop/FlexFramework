using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FlexFramework.Util;

namespace FlexFramework.Core.Data;

public enum VertexAttributeType
{
    Byte,
    UByte,
    Short,
    UShort,
    Int,
    UInt,
    Float,
    Double
}

public enum VertexAttributeIntent
{
    Position,
    Normal,
    Tangent,
    Color,
    TexCoord0,
    TexCoord1,
    TexCoord2,
    TexCoord3,
    BoneWeight,
    BoneIndex
}

public struct VertexAttribute
{
    public VertexAttributeIntent Intent { get; set; }
    public VertexAttributeType Type { get; set; }
    public int Size { get; set; }
    public int Offset { get; set; }
    
    public VertexAttribute(VertexAttributeIntent intent, VertexAttributeType type, int size, int offset)
    {
        Intent = intent;
        Type = type;
        Size = size;
        Offset = offset;
    }
}

public class VertexAttributeAttribute : Attribute
{
    public VertexAttributeIntent Intent { get; set; }
    public VertexAttributeType Type { get; set; }
    public int Size { get; set; }
    
    public VertexAttributeAttribute(VertexAttributeIntent intent, VertexAttributeType type, int size)
    {
        Intent = intent;
        Type = type;
        Size = size;
    }
}

public class VertexLayout
{
    public int Stride { get; }
    public ReadOnlySpan<VertexAttribute> Attributes => attributes.AsSpan();
    public Hash128 Hash { get; }

    private readonly VertexAttribute[] attributes;

    public VertexLayout(int stride, params VertexAttribute[] attributes)
    {
        Stride = stride;
        this.attributes = attributes;
        
        Span<byte> attributeData = stackalloc byte[Unsafe.SizeOf<VertexAttribute>() * attributes.Length];
        MemoryMarshal.Cast<VertexAttribute, byte>(attributes).CopyTo(attributeData);
        Hash = HashUtil.Hash(attributeData);
    }
    
    public VertexLayout DeepCopy()
    {
        return new VertexLayout(Stride, attributes.ToArray());
    }

    public static VertexLayout GetLayout<T>() where T : unmanaged
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var attributes = new List<VertexAttribute>();
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<VertexAttributeAttribute>();
            if (attribute == null)
                continue;
            var offset = (int) Marshal.OffsetOf(type, field.Name);
            attributes.Add(new VertexAttribute(attribute.Intent, attribute.Type, attribute.Size, offset));
        }
        return new VertexLayout(Unsafe.SizeOf<T>(), attributes.ToArray());
    }
}

public class Mesh<T> : DataObject where T : unmanaged
{
    private struct ReadOnlyMesh : IMeshView
    {
        public IBufferView VertexBuffer => mesh.VertexBuffer.ReadOnly;
        public IBufferView? IndexBuffer => mesh.IndexBuffer?.ReadOnly;
        public int VerticesCount => mesh.VerticesCount;
        public int IndicesCount => mesh.IndicesCount;
        public int VertexSize => mesh.VertexSize;
        public VertexLayout VertexLayout => mesh.VertexLayout;

        private readonly Mesh<T> mesh;
        
        public ReadOnlyMesh(Mesh<T> mesh)
        {
            this.mesh = mesh;
        }
    }
    
    public Buffer VertexBuffer => vertexBuffer;
    public Buffer? IndexBuffer => indexBuffer;
    public int VerticesCount => verticesCount;
    public int IndicesCount => indicesCount;
    public int VertexSize => Unsafe.SizeOf<T>();
    public VertexLayout VertexLayout { get; }
    public IMeshView ReadOnly => new ReadOnlyMesh(this);

    private readonly Buffer vertexBuffer = new Buffer();
    private Buffer? indexBuffer = null;
    private int verticesCount = 0;
    private int indicesCount = 0;

    public Mesh(string name, VertexLayout? vertexLayout = null) : base(name)
    {
        // If vertex layout is not specified, generate it from field attributes
        VertexLayout = vertexLayout ?? VertexLayout.GetLayout<T>();
    }

    public Mesh(string name, ReadOnlySpan<T> vertices, VertexLayout? vertexLayout = null) : this(name, vertexLayout)
    {
        SetData(vertices, null);
    }
    
    public Mesh(string name, ReadOnlySpan<T> vertices, ReadOnlySpan<int> indices, VertexLayout? vertexLayout = null) : this(name, vertexLayout)
    {
        SetData(vertices, indices);
    }

    public void SetData(ReadOnlySpan<T> vertices, ReadOnlySpan<int> indices)
    {
        vertexBuffer.SetData(vertices);
        verticesCount = vertices.Length;
        
        if (indices.Length > 0)
        {
            indexBuffer = new Buffer();
            indexBuffer.SetData(indices);
            indicesCount = indices.Length;
        }
        else
        {
            indexBuffer = null;
        }
    }
    
    public T GetVertex(int index)
    {
        if (index < 0 || index >= verticesCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        int size = Unsafe.SizeOf<T>();
        ReadOnlySpan<byte> data = vertexBuffer.Data;
        return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(data.Slice(index * size, size)));
    }
    
    public int GetIndex(int index)
    {
        if (index < 0 || index >= indicesCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        ReadOnlySpan<byte> data = indexBuffer!.Data;
        return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(data.Slice(index * sizeof(int), sizeof(int))));
    }
}