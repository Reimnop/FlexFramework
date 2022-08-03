namespace FlexFramework.Rendering.Data;

public interface IGpuObject : IDisposable
{
    int Handle { get; }
    string Name { get; }
}