namespace FlexFramework.Text;

/// <summary>
/// Represents the bounds of a line of text.
/// </summary>
public class LineBounds
{
    public int Bottom { get; }
    public int Top { get; }
    public IReadOnlyList<int> CharacterPositions => characterPositions;
    public IReadOnlyList<int> CharacterIndices => characterIndices;

    private readonly int[] characterPositions;
    private readonly int[] characterIndices;
    
    public LineBounds(int top, int bottom, IEnumerable<int> selectablePositions, IEnumerable<int> selectableIndices)
    {
        Top = top;
        Bottom = bottom;
        this.characterPositions = selectablePositions.ToArray();
        this.characterIndices = selectableIndices.ToArray();
    }
}