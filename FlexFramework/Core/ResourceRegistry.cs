namespace FlexFramework.Core;

public class ResourceRegistry : IDisposable
{
    private readonly List<object> resources = new List<object>();

    public ResourceLocation<T> Register<T>(T resource)
    {
        if (resource is null)
        {
            throw new NullReferenceException();
        }
        
        resources.Add(resource);
        return new ResourceLocation<T>(resources.Count - 1);
    }

    public object GetResource(IResourceLocation location)
    {
        return resources[location.Id];
    }
    
    public T GetResource<T>(ResourceLocation<T> location)
    {
        return (T) resources[location.Id];
    }

    public void Dispose()
    {
        foreach (object resource in resources)
        {
            if (resource is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}