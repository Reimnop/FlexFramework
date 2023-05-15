using System.Numerics;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Logging;
using OpenTK.Graphics.OpenGL4;
using SharpFont;
using Textwriter;

namespace FlexFramework.Core;

public class TextAssets : IDisposable
{
    private const int AtlasWidth = 1024;
    
    public IReadOnlyList<Font> Fonts => fonts;
    public IReadOnlyList<Texture2D> AtlasTextures => atlasTextures;

    private readonly FlexFrameworkMain engine;
    private readonly Library freetype;

    private readonly Dictionary<string, int> fontIndices = new();
    private readonly List<Font> fonts = new();
    private readonly List<Texture2D> atlasTextures = new();

    public TextAssets(FlexFrameworkMain engine)
    {
        this.engine = engine;
        freetype = new Library();
    }

    public Font this[string name]
    {
        get
        {
            if (!fontIndices.TryGetValue(name, out int fontIndex))
            {
                throw new KeyNotFoundException($"The font '{name}' does not exist!");
            }

            return Fonts[fontIndex];
        }
    }
    
    public void LoadFont(string path, string name, int size)
    {
        engine.LogMessage(this, Severity.Info, null, $"Loading font [{name}] from [{path}], size [{size}]");
        
        var font = new Font(freetype, path, size, AtlasWidth);
        fontIndices.Add(name, fonts.Count);
        fonts.Add(font);

        AtlasTexture<Vector3> atlas = font.Atlas;
        var texture = new Texture2D(name, atlas.Texture.Width, atlas.Texture.Height, SizedInternalFormat.Rgb32f);
        texture.SetData<Vector3>(atlas.Texture.Pixels, PixelFormat.Rgb, PixelType.Float);
        texture.SetMinFilter(TextureMinFilter.Linear);
        texture.SetMagFilter(TextureMagFilter.Linear);
        
        atlasTextures.Add(texture);
    }

    public void Dispose()
    {
        foreach (Font font in Fonts)
        {
            font.Dispose();
        }
        
        foreach (Texture2D texture in AtlasTextures)
        {
            texture.Dispose();
        }
        
        freetype.Dispose();
    }
}
