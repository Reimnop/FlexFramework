namespace FlexFramework.Text;

/// <summary>
/// Represents a glyph's metrics, such as its advance and bearing.
/// </summary>
public struct GlyphMetrics
{
    public int Width { get; }
    public int Height { get; }
    public int AdvanceX { get; }
    public int AdvanceY { get; }
    public int HorizontalBearingX { get; }
    public int HorizontalBearingY { get; }
    public int VerticalBearingX { get; }
    public int VerticalBearingY { get; }
    
    public GlyphMetrics(int width, int height, int advanceX, int advanceY, int horizontalBearingX, int horizontalBearingY, int verticalBearingX, int verticalBearingY)
    {
        Width = width;
        Height = height;
        AdvanceX = advanceX;
        AdvanceY = advanceY;
        HorizontalBearingX = horizontalBearingX;
        HorizontalBearingY = horizontalBearingY;
        VerticalBearingX = verticalBearingX;
        VerticalBearingY = verticalBearingY;
    }
}