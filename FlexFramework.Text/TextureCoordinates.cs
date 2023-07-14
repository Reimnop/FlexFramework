namespace FlexFramework.Text;

/// <summary>
/// Represents a glyph's position in a texture.
/// </summary>
public struct TextureCoordinates
{
    public float MinX { get; }
    public float MinY { get; }
    public float MaxX { get; }
    public float MaxY { get; }
    
    public TextureCoordinates(float minX, float minY, float maxX, float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }
}