using ManagedBass;

namespace FlexFramework.Core.Audio;

public class AudioStream
{
    public bool Playing => Bass.ChannelIsActive(Handle) == PlaybackState.Playing;
    public float Position
    {
        get => (float) Bass.ChannelBytes2Seconds(Handle, Bass.ChannelGetPosition(Handle));
        set => Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, value));
    }
    
    internal int Handle { get; }
    internal AudioSource Source { get; }

    internal AudioStream(AudioSource source, int handle)
    {
        Source = source;
        Handle = handle;
    }
    
    public void Play(bool restart = false)
    {
        Bass.ChannelPlay(Handle, restart);
    }
    
    public void Pause()
    {
        Bass.ChannelPause(Handle);
    }
    
    public void Stop()
    {
        Bass.ChannelStop(Handle);
    }

    public void Dispose()
    {
        Bass.StreamFree(Handle);
        Source.Remove(this);
    }
    
    internal void DisposeDontRemove()
    {
        Bass.StreamFree(Handle);
    }
}