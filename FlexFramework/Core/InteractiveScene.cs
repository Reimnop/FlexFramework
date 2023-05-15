using System.Diagnostics;
using FlexFramework.Core.Entities;

namespace FlexFramework.Core;

public abstract class InteractiveScene : Scene, IDisposable
{
    private readonly Dictionary<Entity, int> entityIndex = new();
    private readonly List<Entity> entities = new();
    private readonly List<bool> states = new();
    
    protected InteractiveScene(FlexFrameworkMain engine) : base(engine)
    {
    }
    
    public override void Update(UpdateArgs args)
    {
        Debug.Assert(entities.Count == states.Count);

        for (int i = 0; i < entities.Count; i++)
        {
            if (!states[i])
            {
                return;
            }
            
            entities[i].Update(args);
        }
    }

    protected T CreateEntity<T>(Func<T> factory) where T : Entity
    {
        return (T) CreateEntity(() => (Entity) factory());
    }

    protected Entity CreateEntity(Func<Entity> factory)
    {
        var entity = factory();
        var index = entities.Count;
        entities.Add(entity);
        states.Add(true);
        entityIndex.Add(entity, index);
        return entity;
    }

    protected void DestroyEntity(Entity entity)
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException("The provided entity is not a member of this scene!", nameof(entity));
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

    protected void SetEntityState(Entity entity, bool enabled)
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException("The provided entity is not a member of this scene!", nameof(entity));
        }

        states[index] = enabled;
    }
    
    protected bool GetEntityState(Entity entity)
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException("The provided entity is not a member of this scene!", nameof(entity));
        }

        return states[index];
    }

    protected void EntityCall<T>(T entity, Action<T> action) where T : Entity
    {
        if (!entityIndex.TryGetValue(entity, out var index))
        {
            throw new ArgumentException("The provided entity is not a member of this scene!", nameof(entity));
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