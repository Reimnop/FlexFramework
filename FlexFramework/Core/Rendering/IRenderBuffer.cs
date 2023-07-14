using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public interface IRenderBuffer
{
    Texture2D Texture { get; }
    Vector2i Size { get; }
    void Resize(Vector2i size);
}