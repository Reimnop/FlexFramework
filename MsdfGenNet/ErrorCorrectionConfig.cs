using System.Runtime.InteropServices;

namespace MsdfGenNet;

public enum ErrorCorrectionMode
{
    Disabled,
    Indiscriminate,
    EdgePriority,
    EdgeOnly
}

public enum DistanceCheckMode {
    DoNotCheckDistance,
    CheckDistanceAtEdge,
    AlwaysCheckDistance
}

[StructLayout(LayoutKind.Sequential)]
public struct ErrorCorrectionConfig
{
    public ErrorCorrectionMode Mode { get; set; }
    public DistanceCheckMode DistanceCheckMode { get; set; }
    public double MinDeviationRatio { get; set; }
    public double MinImproveRatio { get; set; }
    public IntPtr Buffer { get; set; } = IntPtr.Zero;
    
    public ErrorCorrectionConfig(ErrorCorrectionMode mode, DistanceCheckMode distanceCheckMode, double minDeviationRatio, double minImproveRatio, IntPtr buffer)
    {
        Mode = mode;
        DistanceCheckMode = distanceCheckMode;
        MinDeviationRatio = minDeviationRatio;
        MinImproveRatio = minImproveRatio;
        Buffer = buffer;
    }
}