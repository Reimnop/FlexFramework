namespace FlexFramework.Core;

public interface IResourceLocation
{
    int Id { get; }
}

public struct ResourceLocation<T> : IResourceLocation
{
    public int Id { get; }
    
    public ResourceLocation(int id)
    {
        Id = id;
    }
}