using FlexFramework.Core.Rendering.Data;

namespace FlexFramework.Core.Rendering;

public interface IGBuffer
{
    Texture2D WorldColor { get; }
    Texture2D WorldNormal { get; }
    Texture2D WorldPosition { get; }
    Texture2D WorldDepth { get; }
}