namespace FlexFramework.Modelling.Animation;

public struct Key<T>
{
    public float Time { get; }
    public T Value { get; }

    public Key(float time, T value)
    {
        Time = time;
        Value = value;
    }
}