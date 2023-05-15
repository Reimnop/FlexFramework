using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct MaterialData
{
    [FieldOffset(0)] public bool UseAlbedoTexture;
    [FieldOffset(4)] public bool UseMetallicTexture;
    [FieldOffset(8)] public bool UseRoughnessTexture;
    [FieldOffset(16)] public Vector3 Albedo;
    [FieldOffset(32)] public float Metallic;
    [FieldOffset(36)] public float Roughness;
}