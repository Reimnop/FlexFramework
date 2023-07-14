namespace FlexFramework.Text;

/// <summary>
/// Represents a glyph's information, such as its metrics and texture coordinates.
/// </summary>
public struct GlyphInfo
{
    public GlyphMetrics Metrics { get; }
    public TextureCoordinates TextureCoordinates { get; }
    
    public GlyphInfo(GlyphMetrics metrics, TextureCoordinates textureCoordinates)
    {
        Metrics = metrics;
        TextureCoordinates = textureCoordinates;
    }
}