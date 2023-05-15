namespace Textwriter;

public struct BuiltGlyph
{
    public Style Style { get; set; }
    public Font Font { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public int Index { get; set; } // Codepoint
    public int TextureIndex { get; set; }
    
    public BuiltGlyph(Style style, Font font, int offsetX, int offsetY, int index, int textureIndex)
    {
        Style = style;
        Font = font;
        OffsetX = offsetX;
        OffsetY = offsetY;
        Index = index;
        TextureIndex = textureIndex;
    }
}