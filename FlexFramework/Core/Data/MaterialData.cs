using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct MaterialData
{
    [FieldOffset(0)] public bool UseAlbedoTexture = false;
    [FieldOffset(4)] public bool UseMetallicTexture = false;
    [FieldOffset(8)] public bool UseRoughnessTexture = false;
    [FieldOffset(16)] public Vector3 Albedo = Vector3.One;
    [FieldOffset(32)] public Vector3 Emissive = Vector3.Zero;
    [FieldOffset(48)] public float Metallic = 0.5f;
    [FieldOffset(52)] public float Roughness = 0.5f;

    public MaterialData()
    {
    }
}