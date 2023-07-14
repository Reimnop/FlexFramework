using System.Runtime.CompilerServices;
using System.Text;

namespace FlexFramework.Text;

/// <summary>
/// Used to deserialize a font from a binary file.
/// </summary>
public static class FontDeserializer
{
    public static Font Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);
        
        var name = reader.ReadString();
        var metrics = ReadFontMetrics(reader);
        var texture = ReadTexture(reader);

        // Read glyphs
        var glyphCount = reader.ReadInt32();
        var glyphs = new Dictionary<char, GlyphInfo>(glyphCount);
        for (var i = 0; i < glyphCount; i++)
        {
            var c = reader.ReadChar();
            var glyph = ReadGlyphInfo(reader);
            glyphs.Add(c, glyph);
        }
        
        // Read kernings
        var kerningCount = reader.ReadInt32();
        var kernings = new Dictionary<(char, char), int>(kerningCount);
        for (var i = 0; i < kerningCount; i++)
        {
            var left = reader.ReadChar();
            var right = reader.ReadChar();
            var kerning = reader.ReadInt32();
            kernings.Add((left, right), kerning);
        }
        
        return new Font(name, metrics, texture, glyphs, kernings);
    }

    private static FontMetrics ReadFontMetrics(BinaryReader reader)
    {
        var size = reader.ReadInt32();
        var height = reader.ReadInt32();
        var ascent = reader.ReadInt32();
        var descent = reader.ReadInt32();
        return new FontMetrics(size, height, ascent, descent);
    }
    
    private static unsafe Texture<Rgb32f> ReadTexture(BinaryReader reader)
    {
        var pixelSize = Unsafe.SizeOf<Rgb32f>();
        
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var pixels = reader.ReadBytes(width * height * pixelSize);
        
        fixed (byte* p = pixels)
        {
            var span = new Span<Rgb32f>(p, pixels.Length / pixelSize);
            return new Texture<Rgb32f>(width, height, span);
        }
    }
    
    private static GlyphInfo ReadGlyphInfo(BinaryReader reader)
    {
        var metrics = ReadGlyphMetrics(reader);
        var textureCoordinates = ReadTextureCoordinates(reader);
        return new GlyphInfo(metrics, textureCoordinates);
    }

    private static GlyphMetrics ReadGlyphMetrics(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var advanceX = reader.ReadInt32();
        var advanceY = reader.ReadInt32();
        var horizontalBearingX = reader.ReadInt32();
        var horizontalBearingY = reader.ReadInt32();
        var verticalBearingX = reader.ReadInt32();
        var verticalBearingY = reader.ReadInt32();
        return new GlyphMetrics(width, height, advanceX, advanceY, horizontalBearingX, horizontalBearingY, verticalBearingX, verticalBearingY);
    }
    
    private static TextureCoordinates ReadTextureCoordinates(BinaryReader reader)
    {
        var minX = reader.ReadSingle();
        var minY = reader.ReadSingle();
        var maxX = reader.ReadSingle();
        var maxY = reader.ReadSingle();
        return new TextureCoordinates(minX, minY, maxX, maxY);
    }
}