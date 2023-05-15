namespace FlexFramework.Core.Rendering;

public class GpuInfo
{
    public string Name { get; }
    public string Vendor { get; }
    public string Version { get; }
    
    public GpuInfo(string name, string vendor, string version)
    {
        Name = name;
        Vendor = vendor;
        Version = version;
    }
}