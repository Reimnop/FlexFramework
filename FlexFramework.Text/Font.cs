namespace FlexFramework.Text;

/// <summary>
/// Represents a font that can be used to render text.
/// </summary>
public class Font
{
    private const char Tofu = (char) 0xFFFD;
    
    public int GlyphCount => glyphs.Count;
    public int KerningCount => kernings.Count;
    
    public string Name { get; }
    public FontMetrics Metrics { get; }
    public Texture<Rgb32f> Texture { get; }

    private readonly Dictionary<char, GlyphInfo> glyphs = new();
    private readonly Dictionary<(char, char), int> kernings = new();

    public Font(
        string name, 
        FontMetrics metrics, 
        Texture<Rgb32f> texture,
        IDictionary<char, GlyphInfo> glyphs,
        IDictionary<(char, char), int> kernings)
    {
        if (!glyphs.ContainsKey(Tofu))
            throw new ArgumentException("Glyphs must contain a tofu glyph.", nameof(glyphs));
        
        Name = name;
        Metrics = metrics;
        Texture = texture.Clone();
        this.glyphs = glyphs.ToDictionary(x => x.Key, x => x.Value);
        this.kernings = kernings.ToDictionary(x => x.Key, x => x.Value);
    }
    
    /// <summary>
    /// Provides a glyph for the specified character.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <returns></returns>
    public GlyphInfo GetGlyph(char c)
    {
        return glyphs.TryGetValue(c, out GlyphInfo glyph) ? glyph : glyphs[Tofu];
    }
    
    public int GetKerning(char left, char right)
    {
        return kernings.TryGetValue((left, right), out int kerning) ? kerning : 0;
    }
    
    public IEnumerable<(char, char, int)> GetKernings()
    {
        return kernings.Select(x => (x.Key.Item1, x.Key.Item2, x.Value));
    }
    
    public IEnumerable<(char, GlyphInfo)> GetGlyphs()
    {
        return glyphs.Select(x => (x.Key, x.Value));
    }
}