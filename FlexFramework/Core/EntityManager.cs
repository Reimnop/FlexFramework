using System.Diagnostics;
using FlexFramework.Core.Entities;

namespace FlexFramework.Core;

public class EntityManager : IUpdateable, IDisposable
{
    private readonly Dictionary<Entity, int> entityIndex = new();
    private readonly List<Entity> entities = new();
    private readonly List<bool> states = new();
    
    public void Update(UpdateArgs args)
    {
        Debug.Assert(entities.Count == states.Count);

        for (int i = 0; i < entities.Count; i++)
        {
            if (!states[i])
            {
                return;
            }

            if (entities[i] is IUpdateable updateable)
            {
                updateable.Update(args);
            }
        }
    }

    public T Create<T>(Func<T> factory) where T : Entity
    {
        return (T) Create(() => (Entity) factory());
    }

    public Entity Create(Func<Entity> factory)
    {
        var entity = factory();
        var index = entities.Count;
        entities.Add(entity);
        states.Add(true);
        entityIndex.Add(entity, index);
        return entity;
    }

    public void Destroy(Entity entity)
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException($"The provided entity is not a member of this {nameof(EntityManager)}!", nameof(entity));
        }

        entityIndex.Remove(entity);
        entities.RemoveAt(index);
        states.RemoveAt(index);
        
        // Dispose entity if possible
        if (entity is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        // Update entity indices
        for (int i = index; i < entities.Count; i++)
        {
            entityIndex[entities[i]] = i;
        }
    }

    public void SetState(Entity entity, bool enabled)
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException($"The provided entity is not a member of this {nameof(EntityManager)}!", nameof(entity));
        }

        states[index] = enabled;
    }
    
    public bool GetState(Entity entity)
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException($"The provided entity is not a member of this {nameof(EntityManager)}!", nameof(entity));
        }

        return states[index];
    }

    public void Invoke<T>(T entity, Action<T> action) where T : Entity
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException($"The provided entity is not a member of this {nameof(EntityManager)}!", nameof(entity));
        }

        if (states[index])
        {
            action(entity);
        }
    }

    public virtual void Dispose()
    {
        // Dispose all entities
        foreach (var disposable in entities.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}