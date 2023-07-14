namespace FlexFramework.Text;

/// <summary>
/// Represents a font's metrics, such as its ascent and descent.
/// </summary>
public struct FontMetrics
{
    public int Size { get; }
    public int Height { get; }
    public int Ascent { get; }
    public int Descent { get; }
    
    public FontMetrics(int size, int height, int ascent, int descent)
    {
        Size = size;
        Height = height;
        Ascent = ascent;
        Descent = descent;
    }
}