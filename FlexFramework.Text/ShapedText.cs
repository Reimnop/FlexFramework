namespace FlexFramework.Text;

/// <summary>
/// Represents a shaped string of text.
/// </summary>
public class ShapedText
{
    public Font Font { get; }
    public IReadOnlyList<GlyphLine> Lines { get; }
    
    public ShapedText(Font font, IEnumerable<GlyphLine> lines)
    {
        Font = font;
        Lines = lines.ToArray();
    }
}