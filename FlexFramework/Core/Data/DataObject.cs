namespace FlexFramework.Core.Data;

public abstract class DataObject
{
    public string Name { get; }
    
    protected DataObject(string name)
    {
        Name = name;
    }
}