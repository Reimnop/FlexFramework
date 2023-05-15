using System.Collections;
using FlexFramework.Core;

namespace FlexFramework.Core.Entities;

public abstract class Entity : IUpdateable
{
    private bool started = false;

    public virtual void Start()
    {
    }

    public virtual void Update(UpdateArgs args)
    {
        if (!started)
        {
            started = true;
            Start();
        }
    }
}