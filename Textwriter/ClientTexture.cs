using System.Runtime.InteropServices;
using SharpFont;

namespace Textwriter;

public class ClientTexture<T> : IClientTexture where T : unmanaged
{
    public int Width { get; }
    public int Height { get; }
    public T[] Pixels { get; }

    public ClientTexture(int width, int height)
    {
        Pixels = new T[width * height];
        Width = width;
        Height = height;
    }

    public T this[int x, int y]
    {
        get => Pixels[y * Width + x];
        set => Pixels[y * Width + x] = value;
    }

    public void WritePartial(ClientTexture<T> texture, int offsetX, int offsetY)
    {
        for (int y = 0; y < texture.Height; y++)
        {
            for (int x = 0; x < texture.Width; x++)
            {
                this[x + offsetX, y + offsetY] = texture[x, y];
            }
        }
    }
}