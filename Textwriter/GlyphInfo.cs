namespace Textwriter;

public struct GlyphInfo
{
    public Font Font { get; set; }
    public int AdvanceX { get; set; }
    public int AdvanceY { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public int Index { get; set; }
    
    public GlyphInfo(Font font, int advanceX, int advanceY, int offsetX, int offsetY, int index)
    {
        Font = font;
        AdvanceX = advanceX;
        AdvanceY = advanceY;
        OffsetX = offsetX;
        OffsetY = offsetY;
        Index = index;
    }
}