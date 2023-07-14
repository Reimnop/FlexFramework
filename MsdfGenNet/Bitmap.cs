using System.Runtime.InteropServices;

namespace MsdfGenNet;

public class Bitmap<T> : IDisposable where T : unmanaged
{
    public int Width { get; }
    public int Height { get; }
    public int NumChannels { get; }
    public ReadOnlySpan<T> Pixels => pixels;
    internal BitmapRef Reference { get; }

    private T[] pixels;
    
    private GCHandle pixelsHandle;

    public Bitmap(int width, int height, int numChannels)
    {
        Width = width;
        Height = height;
        NumChannels = numChannels;
        pixels = new T[width * height * numChannels];
        
        pixelsHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        Reference = new BitmapRef(pixelsHandle.AddrOfPinnedObject(), width, height);
    }

    public Span<T> this[int x, int y]
    {
        get => pixels.AsSpan((y * Width + x) * NumChannels, NumChannels);
        set => value.CopyTo(pixels.AsSpan((y * Width + x) * NumChannels, NumChannels));
    }

    public void Dispose()
    {
        pixelsHandle.Free();
    }
}