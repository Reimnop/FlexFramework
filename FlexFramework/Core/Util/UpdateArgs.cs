namespace FlexFramework.Core.Util;

public struct UpdateArgs
{
    public double Time { get; }
    public double DeltaTime { get; }

    public UpdateArgs(double time, double deltaTime)
    {
        Time = time;
        DeltaTime = deltaTime;
    }
}