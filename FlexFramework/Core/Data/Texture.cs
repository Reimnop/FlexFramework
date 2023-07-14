using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public enum PixelFormat
{
    Rgba8,
    Rgba16f,
    Rgba16i,
    Rgba32f,
    Rgba32i,
    Rgb8,
    Rgb16f,
    Rgb16i,
    Rgb32f,
    Rgb32i,
    R8,
    R16f,
    R16i,
    R32f,
    R32i
}

public class Texture : DataObject
{
    private struct ReadOnlyTexture : ITextureView
    {
        public string Name => texture.Name;
        public int Width => texture.Width;
        public int Height => texture.Height;
        public PixelFormat Format => texture.Format;
        public IBufferView Data => texture.Data.ReadOnly;

        private readonly Texture texture;

        public ReadOnlyTexture(Texture texture)
        {
            this.texture = texture;
        }
    }
    
    public int Width { get; }
    public int Height { get; }
    public PixelFormat Format { get; }
    public Buffer Data { get; }
    public ITextureView ReadOnly => new ReadOnlyTexture(this);
    
    private bool readOnly = false;
    
    public Texture(string name, int width, int height, PixelFormat format) : base(name)
    {
        Width = width;
        Height = height;
        Format = format;
        Data = new Buffer(width * height * GetPixelSize(format));
    }
    
    public Texture(string name, int width, int height, PixelFormat format, ReadOnlySpan<byte> data) : this(name, width, height, format)
    {
        SetData(data);
    }
    
    public static Texture FromFile(string name, string path)
    {
        using var image = Image.Load<Rgba32>(path);
        Rgba32[] pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
        
        var texture = new Texture(name, image.Width, image.Height, PixelFormat.Rgba8);
        texture.SetData<Rgba32>(pixels);
        return texture;
    }
    
    public Texture SetReadOnly()
    {
        readOnly = true;
        Data.SetReadOnly();
        return this;
    }

    public Texture SetData<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        if (readOnly)
            throw new InvalidOperationException("Cannot modify read-only texture!");
        
        var dataSize = data.Length * Unsafe.SizeOf<T>();
        var requiredSize = Width * Height * GetPixelSize(Format);
        if (dataSize != requiredSize)
            throw new ArgumentException($"Data size does not match texture size (expected {requiredSize}, got {dataSize})");
        Data.SetData(data);

        return this;
    }

    public static int GetPixelSize(PixelFormat format)
    {
        return format switch
        {
            PixelFormat.Rgba8 => 4,
            PixelFormat.Rgba16f => 8,
            PixelFormat.Rgba16i => 8,
            PixelFormat.Rgba32f => 16,
            PixelFormat.Rgba32i => 16,
            PixelFormat.Rgb8 => 3,
            PixelFormat.Rgb16f => 6,
            PixelFormat.Rgb16i => 6,
            PixelFormat.Rgb32f => 12,
            PixelFormat.Rgb32i => 12,
            PixelFormat.R8 => 1,
            PixelFormat.R16f => 2,
            PixelFormat.R16i => 2,
            PixelFormat.R32f => 4,
            PixelFormat.R32i => 4,
            _ => throw new ArgumentException(null, nameof(format))
        };
    }
}