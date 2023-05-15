namespace FlexFramework.Util;

// A simple GC implementation for OpenGL objects
public class GarbageCollector<TInput, TOutput> : IDisposable where TOutput : IDisposable
{
    private class AliveObjectReference
    {
        public TOutput Object { get; }
        public bool Used { get; set; }
        
        public AliveObjectReference(TOutput obj)
        {
            Object = obj;
            Used = true;
        }
    }
    
    private readonly Dictionary<Hash128, AliveObjectReference> allocated = new();
    private readonly Func<TInput, Hash128> hashFunc;
    private readonly Func<TInput, TOutput> factoryFunc;
    
    public GarbageCollector(Func<TInput, Hash128> hashFunc, Func<TInput, TOutput> factoryFunc)
    {
        this.hashFunc = hashFunc;
        this.factoryFunc = factoryFunc;
    }

    public TOutput GetOrAllocate(TInput input)
    {
        Hash128 hash = hashFunc(input);
        if (allocated.TryGetValue(hash, out AliveObjectReference? reference))
        {
            reference.Used = true;
            return reference.Object;
        }

        TOutput obj = factoryFunc(input);
        allocated.Add(hash, new AliveObjectReference(obj));
        return obj;
    }
    
    public void Sweep()
    {
        List<Hash128> toClean = new List<Hash128>();
        foreach (var (hash, reference) in allocated)
        {
            if (reference.Used)
                reference.Used = false;
            else
                toClean.Add(hash);
        }
        
        foreach (var hash in toClean)
        {
            AliveObjectReference reference = allocated[hash];
            reference.Object.Dispose();
            allocated.Remove(hash);
        }
    }

    public void Dispose()
    {
        foreach (var (_, reference) in allocated)
            reference.Object.Dispose();
    }
}