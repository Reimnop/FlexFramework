using System.Runtime.InteropServices;
using NVorbis;

namespace FlexFramework.Core.Audio;

public class VorbisAudioStream : AudioStream
{
    public override float Length => vorbis.TotalTime.Seconds;
    public override int Channels => vorbis.Channels;
    public override int BytesPerSample => 4;
    public override int SampleRate => vorbis.SampleRate;
    public override long SampleCount => vorbis.TotalSamples;
    public override long SamplePosition => vorbis.SamplePosition;

    private readonly VorbisReader vorbis;
    private readonly float[] readBuffer;
    private readonly byte[] copyBuffer;

    public VorbisAudioStream(string path) : this(File.OpenRead(path))
    {
    }

    public VorbisAudioStream(Stream stream)
    {
        vorbis = new VorbisReader(stream);
        
        // buffer one/20 second of audio
        readBuffer = new float[vorbis.Channels * vorbis.SampleRate / 20];
        copyBuffer = new byte[vorbis.Channels * vorbis.SampleRate * sizeof(float) / 20];
    }

    public override void Seek(long position, SeekOrigin seekOrigin = SeekOrigin.Begin)
    {
        vorbis.SeekTo(position, seekOrigin);
    }

    public override bool ShouldQueueBuffers()
    {
        return SamplePosition < SampleCount;
    }

    public override bool NextBuffer(out Span<byte> data)
    {
        int readLength = vorbis.ReadSamples(readBuffer);
        
        if (readLength == 0)
        {
            data = null;
            return false;
        }

        GCHandle handle = GCHandle.Alloc(copyBuffer, GCHandleType.Pinned);
        Marshal.Copy(readBuffer, 0, handle.AddrOfPinnedObject(), readLength);
        handle.Free();

        data = new Span<byte>(copyBuffer, 0, readLength * sizeof(float));
        return true;
    }

    public override void Dispose()
    {
        vorbis.Dispose();
    }
}