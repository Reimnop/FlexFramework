using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace FlexFramework.Rendering.Data;

public class Texture2D : IGpuObject
{
    public int Handle { get; }
    public string Name { get; }

    public int Width { get; }
    public int Height { get; }

    public Texture2D(string name, int width, int height, SizedInternalFormat internalFormat)
    {
        Name = name;
        Width = width;
        Height = height;
        
        GL.CreateTextures(TextureTarget.Texture2D, 1, out int handle);
        GL.TextureStorage2D(handle, 1, internalFormat, width, height);
        
        GL.ObjectLabel(ObjectLabelIdentifier.Texture, handle, name.Length, name);
        Handle = handle;
    }

    public static Texture2D FromFile(string name, string path)
    {
        using FileStream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        Texture2D texture2D = new Texture2D(name, result.Width, result.Height, SizedInternalFormat.Rgba8);
        texture2D.LoadData(result.Data, PixelFormat.Rgba, PixelType.UnsignedByte);
        texture2D.SetMinFilter(TextureMinFilter.Linear);
        texture2D.SetMagFilter(TextureMagFilter.Linear);
        return texture2D;
    }

    public void LoadData<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType) where T : struct
    {
        GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, pixelFormat, pixelType, data);
    }
    
    public void LoadDataPartial<T>(T[] data, int x, int y, int width, int height, PixelFormat pixelFormat, PixelType pixelType) where T : struct
    {
        GL.TextureSubImage2D(Handle, 0, x, y, width, height, pixelFormat, pixelType, data);
    }

    public void SetMinFilter(TextureMinFilter filter)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int) filter);
    }
    
    public void SetMagFilter(TextureMagFilter filter)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int) filter);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}