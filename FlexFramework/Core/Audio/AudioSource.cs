using ManagedBass;

namespace FlexFramework.Core.Audio;

public class AudioSource : IDisposable
{
    public float Gain
    {
        get => gain;
        set
        {
            gain = value;
            foreach (var stream in streams)
            {
                Bass.ChannelSetAttribute(stream.Handle, ChannelAttribute.Volume, value);
            }
        }
    }

    public float Pitch
    {
        get => pitch;
        set
        {
            pitch = value;
            foreach (var stream in streams)
            {
                Bass.ChannelSetAttribute(stream.Handle, ChannelAttribute.Pitch, value);
            }
        }
    }

    private float gain = 1.0f;
    private float pitch = 1.0f;
    
    private readonly List<AudioStream> streams = new();
    
    public AudioStream CreateStream(AudioData data, bool loop = false)
    {
        var handle = Bass.CreateStream(data.Path, 0, 0, loop ? BassFlags.Loop : BassFlags.Default);
        Bass.ChannelSetAttribute(handle, ChannelAttribute.Volume, Gain);
        Bass.ChannelSetAttribute(handle, ChannelAttribute.Pitch, Pitch);
        streams.Add(new AudioStream(this, handle));
        return new AudioStream(this, handle);
    }

    internal void Remove(AudioStream stream)
    {
        streams.Remove(stream);
    }

    public void Dispose()
    {
        foreach (var stream in streams)
        {
            stream.DisposeDontRemove();
        }
    }
}