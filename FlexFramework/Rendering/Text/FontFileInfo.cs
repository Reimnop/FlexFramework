namespace FlexFramework.Rendering.Text;

public struct FontFileInfo
{
    public string Name { get; }
    public string Path { get; }

    public FontFileInfo(string name, string path)
    {
        Name = name;
        Path = path;
    }
}