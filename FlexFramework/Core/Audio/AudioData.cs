namespace FlexFramework.Core.Audio;

// Literally just a wrapper for audio paths
// TODO: Add support for loading audio from memory
public class AudioData
{
    internal string Path { get; }
    
    public AudioData(string path)
    {
        Path = path;
    }
}