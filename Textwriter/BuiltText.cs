using System.Collections;

namespace Textwriter;

public class BuiltText : IEnumerable
{
    public BuiltGlyph[] Glyphs { get; }

    public BuiltText(IEnumerable<BuiltGlyph> glyphs)
    {
        Glyphs = glyphs.ToArray();
    }
    
    public IEnumerator GetEnumerator()
    {
        return Glyphs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}