using FlexFramework.Logging;
using OpenTK.Audio.OpenAL;

namespace FlexFramework.Core.Audio;

public class AudioManager : IDisposable
{
    private readonly ALDevice device;
    private readonly ALContext context;
    
    public AudioManager()
    {
        string deviceName = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);
        device = ALC.OpenDevice(deviceName);
        context = ALC.CreateContext(device, (int[]) null);
        ALC.MakeContextCurrent(context);
    }
    
    public void Dispose()
    {
        ALC.DestroyContext(context);
        ALC.CloseDevice(device);
    }
}