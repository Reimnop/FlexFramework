namespace FlexFramework.Text;

/// <summary>
/// Represents a texture, which is a 2D array of pixels.
/// </summary>
/// <typeparam name="T">The type of each pixel.</typeparam>
public class Texture<T> where T : unmanaged
{
    public int Width { get; }
    public int Height { get; }
    public T[] Pixels { get; }

    public Texture(int width, int height)
    {
        Pixels = new T[width * height];
        Width = width;
        Height = height;
    }
    
    public Texture(int width, int height, ReadOnlySpan<T> pixels)
    {
        Pixels = new T[width * height];
        Width = width;
        Height = height;
        
        if (pixels.Length != Pixels.Length)
            throw new ArgumentException("The number of pixels must match the width and height of the texture.", nameof(pixels));
        
        pixels.CopyTo(Pixels);
    }

    public T this[int x, int y]
    {
        get => Pixels[y * Width + x];
        set => Pixels[y * Width + x] = value;
    }
    
    public Texture<T> Clone()
    {
        var clone = new Texture<T>(Width, Height);
        Array.Copy(Pixels, clone.Pixels, Pixels.Length);
        return clone;
    }

    public void WritePartial(Texture<T> texture, int offsetX, int offsetY)
    {
        int sourceRowLength = texture.Width;
        int destRowLength = Width;

        int sourceOffset = 0;
        int destOffset = offsetY * destRowLength + offsetX;
        int rowsToCopy = Math.Min(texture.Height, Height - offsetY);

        for (int i = 0; i < rowsToCopy; i++)
        {
            Array.Copy(texture.Pixels, sourceOffset, Pixels, destOffset, sourceRowLength);
            sourceOffset += sourceRowLength;
            destOffset += destRowLength;
        }
    }
}