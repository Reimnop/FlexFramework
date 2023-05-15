using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public interface ILighting
{
    Vector3 AmbientLight { get; set; }
    DirectionalLight? DirectionalLight { get; set; }
}