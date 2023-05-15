namespace FlexFramework.Core.Audio;

public abstract class AudioStream : IDisposable
{
    public abstract float Length { get; }
    public abstract int Channels { get; }
    public abstract int BytesPerSample { get; }
    public abstract int SampleRate { get; }
    public abstract long SampleCount { get; }
    public abstract long SamplePosition { get; }

    public abstract void Seek(long position, SeekOrigin seekOrigin = SeekOrigin.Begin);
    public abstract bool ShouldQueueBuffers();
    public abstract bool NextBuffer(out Span<byte> data);
    public abstract void Dispose();
}