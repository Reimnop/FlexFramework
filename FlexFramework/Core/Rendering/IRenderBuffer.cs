using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public interface IRenderBuffer
{
    Vector2i Size { get; }
    void Resize(Vector2i size);
    void BlitToBackBuffer(Vector2i size);
}