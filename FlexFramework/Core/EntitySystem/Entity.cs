using FlexFramework.Core.Util;

namespace FlexFramework.Core.EntitySystem;

public abstract class Entity : IDisposable
{
    public abstract void Update(UpdateArgs args);
    public abstract void Dispose();
}