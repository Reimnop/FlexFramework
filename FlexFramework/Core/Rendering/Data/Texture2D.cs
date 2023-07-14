using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpEXR;
using PixelType = OpenTK.Graphics.OpenGL4.PixelType;

namespace FlexFramework.Core.Rendering.Data;

public class Texture2D : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public int Width { get; }
    public int Height { get; }

    public Texture2D(string name, int width, int height, SizedInternalFormat internalFormat, int samples = 0)
    {
        Name = name;
        Width = width;
        Height = height;

        GL.CreateTextures(samples > 0 ? TextureTarget.Texture2DMultisample : TextureTarget.Texture2D, 1, out int handle);

        if (samples > 0)
            GL.TextureStorage2DMultisample(handle, samples, internalFormat, width, height, true);
        else
            GL.TextureStorage2D(handle, 1, internalFormat, width, height);

        GL.ObjectLabel(ObjectLabelIdentifier.Texture, handle, name.Length, name);
        Handle = handle;
    }

    public static Texture2D FromFile(string name, string path)
    {
        using var stream = File.OpenRead(path);
        return FromStream(name, stream);
    }
    
    public static Texture2D FromStream(string name, Stream stream)
    {
        using var image = Image.Load<Rgba32>(stream);
        var pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
        
        var texture2D = new Texture2D(name, image.Width, image.Height, SizedInternalFormat.Rgba8);
        texture2D.SetData<Rgba32>(pixels, PixelFormat.Rgba, PixelType.UnsignedByte);
        return texture2D;
    }

    public unsafe void SetData<T>(ReadOnlySpan<T> data, PixelFormat pixelFormat, PixelType pixelType) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, pixelFormat, pixelType, (IntPtr) ptr);
        }
    }
    
    public unsafe void LoadDataPartial<T>(ReadOnlySpan<T> data, int x, int y, int width, int height, PixelFormat pixelFormat, PixelType pixelType) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            GL.TextureSubImage2D(Handle, 0, x, y, width, height, pixelFormat, pixelType, (IntPtr) ptr);
        }
    }

    public void GenerateMipmap()
    {
        GL.GenerateTextureMipmap(Handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}