using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace FlexFramework.Audio;

public class AudioSource : IDisposable
{
    public int Handle { get; }

    private float gain = 1.0f;
    private float pitch = 1.0f;
    private bool loop = true;

    public double Gain
    {
        get => gain;
        set
        {
            gain = (float) value;
            AL.Source(Handle, ALSourcef.Gain, gain);
        }
    }

    public double Pitch
    {
        get => pitch;
        set
        {
            pitch = (float) value;
            AL.Source(Handle, ALSourcef.Pitch, pitch);
        }
    }

    public bool Looping
    {
        get => loop;
        set
        {
            if (loop == value)
            {
                return;
            }

            loop = value;
            AL.Source(Handle, ALSourceb.Looping, value);
        }
    }

    public bool Playing => AL.GetSourceState(Handle) == ALSourceState.Playing;

    public Vector3d Position
    {
        get
        {
            AL.GetSource(Handle, ALSource3f.Position, out Vector3 position);
            return position;
        }
        set
        {
            Vector3 position = (Vector3) value;
            AL.Source(Handle, ALSource3f.Position, ref position);
        }
    }

    private AudioClip? audioClip = null;

    public AudioClip? Clip
    {
        get => audioClip;
        set
        {
            audioClip = value;

            if (audioClip == null)
            {
                return;
            }
            
            AL.Source(Handle, ALSourcei.Buffer, audioClip.Handle);
        }
    }

    public AudioSource()
    {
        Handle = AL.GenSource();
        AL.Source(Handle, ALSourcef.Gain, gain);
        AL.Source(Handle, ALSourcef.Pitch, pitch);
        AL.Source(Handle, ALSourceb.Looping, loop);
    }

    public void Play()
    {
        AL.SourcePlay(Handle);
    }

    public void Pause()
    {
        AL.SourcePause(Handle);
    }

    public void Stop()
    {
        AL.SourceStop(Handle);
    }

    public double GetPlayPosition()
    {
        AL.GetSource(Handle, ALGetSourcei.SampleOffset, out int bPosition);
        return bPosition / (double) Clip.SampleRate;
    }
    
    public void Dispose()
    {
        AL.SourceStop(Handle);
        AL.DeleteSource(Handle);
    }
}