using System.Drawing;
using System.Net.Mime;
using System.Numerics;

namespace Textwriter;

public class AtlasTexture<T> where T : unmanaged
{
    private const int Gap = 1;
    
    public ClientTexture<T> Texture { get; }

    private int ptrX = 0;
    private int ptrY = 0;
    private int maxY = 0;

    public AtlasTexture(int width, int height)
    {
        Texture = new ClientTexture<T>(width, height);
    }

    public UvInfo AddGlyphTexture(ClientTexture<T> texture)
    {
        // overflow check
        if (ptrX + texture.Width > Texture.Width)
        {
            ptrY += maxY + Gap;
            maxY = 0;
            ptrX = 0;
        }

        if (ptrY + texture.Height > Texture.Height)
        {
            throw new Exception("Could not fit texture onto atlas!");
        }
        
        Texture.WritePartial(texture, ptrX, ptrY);
        UvInfo uvInfo = new UvInfo
        {
            Min = new Vector2(ptrX / (float)Texture.Width, ptrY / (float)Texture.Height),
            Max = new Vector2((ptrX + texture.Width) / (float)Texture.Width, (ptrY + texture.Height) / (float)Texture.Height)
        };
        ptrX += texture.Width + Gap;
        maxY = Math.Max(maxY, texture.Height);
        return uvInfo;
    }
}