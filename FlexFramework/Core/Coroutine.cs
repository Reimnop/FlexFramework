using System.Collections;

namespace FlexFramework.Core;

public class Coroutine
{
    public IEnumerator InternalRoutine { get; }

    public Coroutine(IEnumerator routine)
    {
        InternalRoutine = routine;
    }
}