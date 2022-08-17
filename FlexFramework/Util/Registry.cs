using System.Collections;
using FlexFramework.Util.Exceptions;

namespace FlexFramework.Util;

public class Registry<TKey, T> : IEnumerable<T>
{
    public int Count
    {
        get
        {
            if (registeredObjects == null)
            {
                throw new RegistryNotFrozenException();
            }
            
            return registeredObjects.Count;
        }   
    }
    
    private List<T>? registeredObjects;
    private Dictionary<TKey, int> keyToId = new Dictionary<TKey, int>();
    
    private List<ObjectFactory<T>>? objectFactories = new List<ObjectFactory<T>>();

    public T this[int id]
    {
        get
        {
            if (registeredObjects == null)
            {
                throw new RegistryNotFrozenException();
            }
            
            return registeredObjects[id];
        }
    }

    public bool HasKey(int id)
    {
        if (registeredObjects == null)
        {
            throw new RegistryNotFrozenException();
        }

        return id < registeredObjects.Count;
    }

    public int GetId(TKey key)
    {
        return keyToId[key];
    }

    public int Register(TKey key, ObjectFactory<T> factory)
    {
        if (registeredObjects != null)
        {
            throw new RegistryFrozenException();
        }
        
        int id = objectFactories.Count;
        objectFactories.Add(factory);
        keyToId.Add(key, id);

        return id;
    }

    public void Freeze()
    {
        if (registeredObjects != null)
        {
            throw new RegistryFrozenException();
        }
        
        registeredObjects = objectFactories
            .Select(x => x())
            .ToList();
        
        objectFactories = null;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (registeredObjects == null)
        {
            throw new RegistryNotFrozenException();
        }
        
        return registeredObjects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}