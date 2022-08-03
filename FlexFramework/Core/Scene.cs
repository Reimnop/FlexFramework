using System.Collections;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;

namespace FlexFramework.Core;

public abstract class Scene : IDisposable
{
    private HashSet<Coroutine> coroutines = new HashSet<Coroutine>();
    private List<Coroutine> finishedCoroutines = new List<Coroutine>();

    private double deltaTime = 0.0;

    protected FlexFrameworkMain Engine { get; private set; }

    internal void SetEngine(FlexFrameworkMain engine)
    {
        Engine = engine;
    }

    internal void UpdateInternal(UpdateArgs args)
    {
        deltaTime = args.DeltaTime;

        foreach (Coroutine coroutine in coroutines)
        {
            if (!MoveNext(coroutine.InternalRoutine))
            {
                finishedCoroutines.Add(coroutine);
            }
        }

        foreach (Coroutine coroutine in finishedCoroutines)
        {
            coroutines.Remove(coroutine);
        }
        
        finishedCoroutines.Clear();
    }

    #region Coroutine Stuff
    
    private bool MoveNext(IEnumerator coroutine)
    {
        if (coroutine.Current is IEnumerator subroutine)
        {
            if (MoveNext(subroutine))
            {
                return true;
            }
        }

        return coroutine.MoveNext();
    }

    protected Coroutine StartCoroutine(IEnumerator enumerator)
    {
        Coroutine coroutine = new Coroutine(enumerator);
        coroutines.Add(coroutine);
        return coroutine;
    }

    protected void StopCoroutine(Coroutine coroutine)
    {
        coroutines.Remove(coroutine);
    }
    
    protected IEnumerator WaitForEndOfFrame()
    {
        yield return null;
    }

    protected IEnumerator WaitForFrames(int frames)
    {
        int i = 0;
        while (i < frames)
        {
            i++;
            yield return null;
        }
    }

    protected IEnumerator WaitForSeconds(double time)
    {
        double t = 0.0;
        while (t < time)
        {
            t += deltaTime;
            yield return null;
        }
    }

    protected IEnumerator WaitUntil(Func<bool> predicate)
    {
        while (!predicate())
        {
            yield return null;
        }
    }
    
    #endregion
    
    public abstract void Init();
    public abstract void Update(UpdateArgs args);
    public abstract void Render(Renderer renderer);
    public abstract void Dispose();
}