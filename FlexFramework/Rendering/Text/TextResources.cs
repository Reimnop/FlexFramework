using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using SharpFont;
using Textwriter;

namespace FlexFramework.Rendering.Text;

public class TextResources : IDisposable
{
    public Font[] Fonts { get; }
    public Texture2D[] FontTextures { get; }

    private readonly Library freetype;
    private readonly Dictionary<string, int> nameToIndex;

    public TextResources(int size, int atlasWidth, params FontFileInfo[] fontFiles)
    {
        freetype = new Library();
        nameToIndex = new Dictionary<string, int>();

        Fonts = new Font[fontFiles.Length];
        for (int i = 0; i < fontFiles.Length; i++)
        {
            Fonts[i] = new Font(freetype, fontFiles[i].Path, size, atlasWidth);
            nameToIndex.Add(fontFiles[i].Name, i);
        }

        List<Texture2D> textures = new List<Texture2D>();
        foreach (Font font in Fonts)
        {
            if (font.GrayscaleAtlas != null)
            {
                AtlasTexture atlasTexture = font.GrayscaleAtlas;
                Texture2D texture = new Texture2D($"{font.FamilyName}-gs", atlasTexture.Texture.Width,
                    atlasTexture.Texture.Height, SizedInternalFormat.R8);
                texture.LoadData(atlasTexture.Texture.Pixels, PixelFormat.Red, PixelType.UnsignedByte);
                texture.SetMinFilter(TextureMinFilter.Linear);
                texture.SetMagFilter(TextureMagFilter.Nearest);
                textures.Add(texture);
            }
            
            if (font.ColoredAtlas != null)
            {
                AtlasTexture atlasTexture = font.ColoredAtlas;
                Texture2D texture = new Texture2D($"{font.FamilyName}-c", atlasTexture.Texture.Width,
                    atlasTexture.Texture.Height, SizedInternalFormat.Rgba8);
                texture.LoadData(atlasTexture.Texture.Pixels, PixelFormat.Bgra, PixelType.UnsignedByte);
                texture.SetMinFilter(TextureMinFilter.Linear);
                texture.SetMagFilter(TextureMagFilter.Nearest);
                textures.Add(texture);
            }
        }

        FontTextures = textures.ToArray();
    }

    public Font GetFont(string name)
    {
        return Fonts[nameToIndex[name]];
    }
    
    public void Dispose()
    {
        foreach (Font font in Fonts)
        {
            font.Dispose();
        }
        
        foreach (Texture2D texture in FontTextures)
        {
            texture.Dispose();
        }
        
        freetype.Dispose();
    }
}