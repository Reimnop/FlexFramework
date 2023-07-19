using FlexFramework.Util.Logging;
using ManagedBass;

namespace FlexFramework.Core.Audio;

public class AudioManager : IDisposable
{
    private readonly ILogger logger;
    
    public AudioManager(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<AudioManager>();

        Bass.Init();
        
        logger.LogInfo("Initialized audio module");
    }
    
    public void Dispose()
    {
        Bass.Free();
    }
}