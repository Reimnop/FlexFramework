namespace FlexFramework.Util;

/// <summary>
/// A wrapper around list that clears the list when it's disposed
/// </summary>
public struct TemporaryList<T> : IDisposable
{
    private readonly List<T> list;
    
    public TemporaryList(List<T> list) 
    {
        this.list = list;
    }

    public static implicit operator TemporaryList<T>(List<T> list)
        => new TemporaryList<T>(list);

    public static implicit operator List<T>(TemporaryList<T> temporaryList) 
        => temporaryList.list;

    public void Dispose()
    {
        list.Clear();
    }
}