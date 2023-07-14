using System.Collections;

namespace FlexFramework.Text;

/// <summary>
/// Represents a line of shaped glyphs.
/// </summary>
public class GlyphLine : IReadOnlyList<ShapedGlyph>
{
    public int Count => glyphs.Length;

    private readonly ShapedGlyph[] glyphs;

    public GlyphLine(IEnumerable<ShapedGlyph> glyphs)
    {
        this.glyphs = glyphs.ToArray();
    }
    
    public ShapedGlyph this[int index] => glyphs[index];
    
    public IEnumerator<ShapedGlyph> GetEnumerator()
    {
        return ((IEnumerable<ShapedGlyph>) glyphs).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}