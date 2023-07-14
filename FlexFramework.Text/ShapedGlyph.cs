namespace FlexFramework.Text;

/// <summary>
/// Represents a shaped glyph, which is a rectangle with a texture coordinate.
/// </summary>
public struct ShapedGlyph
{
    public int MinPositionX { get; }
    public int MinPositionY { get; }
    public int MaxPositionX { get; }
    public int MaxPositionY { get; }
    public float MinTextureCoordinateX { get; }
    public float MinTextureCoordinateY { get; }
    public float MaxTextureCoordinateX { get; }
    public float MaxTextureCoordinateY { get; }
    
    public ShapedGlyph(int minPositionX, int minPositionY, int maxPositionX, int maxPositionY, float minTextureCoordinateX, float minTextureCoordinateY, float maxTextureCoordinateX, float maxTextureCoordinateY)
    {
        MinPositionX = minPositionX;
        MinPositionY = minPositionY;
        MaxPositionX = maxPositionX;
        MaxPositionY = maxPositionY;
        MinTextureCoordinateX = minTextureCoordinateX;
        MinTextureCoordinateY = minTextureCoordinateY;
        MaxTextureCoordinateX = maxTextureCoordinateX;
        MaxTextureCoordinateY = maxTextureCoordinateY;
    }
}