namespace FlexFramework.Core;

public struct UpdateArgs
{
    public float Time { get; }
    public float DeltaTime { get; }

    public UpdateArgs(float time, float deltaTime)
    {
        Time = time;
        DeltaTime = deltaTime;
    }
}