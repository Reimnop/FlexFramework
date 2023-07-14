namespace FlexFramework.Text;

/// <summary>
/// Represents the bounds of a block of text.
/// </summary>
public class TextBounds
{
    public IReadOnlyList<LineBounds> Lines { get; }
    
    // Outer bounding box
    public int MinX { get; }
    public int MinY { get; }
    public int MaxX { get; }
    public int MaxY { get; }
    
    public TextBounds(IEnumerable<LineBounds> lines)
    {
        Lines = lines.ToList();

        MinX = Lines.Select(line => line.CharacterPositions).Min(x => x.Min());
        MinY = Lines.Select(line => line.Top).Min();
        MaxX = Lines.Select(line => line.CharacterPositions).Max(x => x.Max());
        MaxY = Lines.Select(line => line.Bottom).Max();
    }
}