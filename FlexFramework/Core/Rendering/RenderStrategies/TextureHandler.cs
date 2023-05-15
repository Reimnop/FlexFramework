using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = FlexFramework.Core.Data.PixelFormat;
using Timer = FlexFramework.Util.Timer;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class TextureHandler
{
    private readonly GarbageCollector<ITextureView, Texture2D> gc;
    private readonly Timer timer;
    
    public TextureHandler()
    {
        gc = new GarbageCollector<ITextureView, Texture2D>(GetHash, CreateTexture);
        timer = new Timer(1.0f, () => gc.Sweep());
    }
    
    public void Update(float deltaTime)
    {
        timer.Update(deltaTime);
    }
    
    public Texture2D GetTexture(ITextureView texture)
    {
        return gc.GetOrAllocate(texture);
    }
    
    private static Hash128 GetHash(ITextureView texture)
    {
        return texture.Data.Hash;
    }
    
    private static Texture2D CreateTexture(ITextureView texture)
    {
        SizedInternalFormat internalFormat = ConvertToSizedInternalFormat(texture.Format);
        Texture2D tex = new("texture", texture.Width, texture.Height, internalFormat);
        tex.SetData(texture.Data.Data, ConvertToPixelFormat(texture.Format), ConvertToPixelType(texture.Format));
        
        return tex;
    }
    
    private static SizedInternalFormat ConvertToSizedInternalFormat(PixelFormat format)
    {
        return format switch
        {
            PixelFormat.Rgba8 => SizedInternalFormat.Rgba8,
            PixelFormat.Rgba16f => SizedInternalFormat.Rgba16f,
            PixelFormat.Rgba16i => SizedInternalFormat.Rgba16i,
            PixelFormat.Rgba32f => SizedInternalFormat.Rgba32f,
            PixelFormat.Rgba32i => SizedInternalFormat.Rgba32i,
            PixelFormat.Rgb8 => SizedInternalFormat.Rgb8,
            PixelFormat.Rgb16f => SizedInternalFormat.Rgb16f,
            PixelFormat.Rgb16i => SizedInternalFormat.Rgb16i,
            PixelFormat.Rgb32f => SizedInternalFormat.Rgb32f,
            PixelFormat.Rgb32i => SizedInternalFormat.Rgb32i,
            PixelFormat.R8 => SizedInternalFormat.R8,
            PixelFormat.R16f => SizedInternalFormat.R16f,
            PixelFormat.R16i => SizedInternalFormat.R16i,
            PixelFormat.R32f => SizedInternalFormat.R32f,
            PixelFormat.R32i => SizedInternalFormat.R32i,
            _ => throw new ArgumentException(nameof(format))
        };
    }
    
    private static OpenTK.Graphics.OpenGL4.PixelFormat ConvertToPixelFormat(PixelFormat format)
    {
        return format switch
        {
            PixelFormat.Rgba8 => OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
            PixelFormat.Rgba16f => OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
            PixelFormat.Rgba16i => OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
            PixelFormat.Rgba32f => OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
            PixelFormat.Rgba32i => OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
            PixelFormat.Rgb8 => OpenTK.Graphics.OpenGL4.PixelFormat.Rgb,
            PixelFormat.Rgb16f => OpenTK.Graphics.OpenGL4.PixelFormat.Rgb,
            PixelFormat.Rgb16i => OpenTK.Graphics.OpenGL4.PixelFormat.Rgb,
            PixelFormat.Rgb32f => OpenTK.Graphics.OpenGL4.PixelFormat.Rgb,
            PixelFormat.Rgb32i => OpenTK.Graphics.OpenGL4.PixelFormat.Rgb,
            PixelFormat.R8 => OpenTK.Graphics.OpenGL4.PixelFormat.Red,
            PixelFormat.R16f => OpenTK.Graphics.OpenGL4.PixelFormat.Red,
            PixelFormat.R16i => OpenTK.Graphics.OpenGL4.PixelFormat.Red,
            PixelFormat.R32f => OpenTK.Graphics.OpenGL4.PixelFormat.Red,
            PixelFormat.R32i => OpenTK.Graphics.OpenGL4.PixelFormat.Red,
            _ => throw new ArgumentException(nameof(format))
        };
    }
    
    private static PixelType ConvertToPixelType(PixelFormat format)
    {
        return format switch
        {
            PixelFormat.Rgba8 => PixelType.UnsignedByte,
            PixelFormat.Rgba16f => PixelType.HalfFloat,
            PixelFormat.Rgba16i => PixelType.Int,
            PixelFormat.Rgba32f => PixelType.Float,
            PixelFormat.Rgba32i => PixelType.Int,
            PixelFormat.Rgb8 => PixelType.UnsignedByte,
            PixelFormat.Rgb16f => PixelType.HalfFloat,
            PixelFormat.Rgb16i => PixelType.Int,
            PixelFormat.Rgb32f => PixelType.Float,
            PixelFormat.Rgb32i => PixelType.Int,
            PixelFormat.R8 => PixelType.UnsignedByte,
            PixelFormat.R16f => PixelType.HalfFloat,
            PixelFormat.R16i => PixelType.Int,
            PixelFormat.R32f => PixelType.Float,
            PixelFormat.R32i => PixelType.Int,
            _ => throw new ArgumentException(nameof(format))
        };
    }
}