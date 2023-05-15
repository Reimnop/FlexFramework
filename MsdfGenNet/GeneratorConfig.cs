using System.Runtime.InteropServices;

namespace MsdfGenNet;

[StructLayout(LayoutKind.Sequential)]
public struct GeneratorConfig
{
    public bool OverlapSupport { get; set; } = true;
    
    public GeneratorConfig(bool overlapSupport)
    {
        OverlapSupport = overlapSupport;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct MSDFGeneratorConfig
{
    public bool OverlapSupport { get; set; } = true;
    public ErrorCorrectionConfig ErrorCorrection { get; set; } = new ErrorCorrectionConfig();
    
    public MSDFGeneratorConfig(bool overlapSupport, ErrorCorrectionConfig errorCorrection)
    {
        OverlapSupport = overlapSupport;
        ErrorCorrection = errorCorrection;
    }
}