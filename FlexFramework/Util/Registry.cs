using System.Collections;
using FlexFramework.Util.Exceptions;

namespace FlexFramework.Util;

public class Registry<T> : IEnumerable<T>
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
    private Dictionary<string, int> keyToId = new Dictionary<string, int>();
    
    private List<ObjectFactory<T>>? objectFactories = new List<ObjectFactory<T>>();

    public T this[int key]
    {
        get
        {
            if (registeredObjects == null)
            {
                throw new RegistryNotFrozenException();
            }
            
            return registeredObjects[key];
        }
    }

    public bool HasKey(int key)
    {
        if (registeredObjects == null)
        {
            throw new RegistryNotFrozenException();
        }

        return key < registeredObjects.Count;
    }

    public int GetId(string key)
    {
        return keyToId[key];
    }

    public int Register(string key, ObjectFactory<T> factory)
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