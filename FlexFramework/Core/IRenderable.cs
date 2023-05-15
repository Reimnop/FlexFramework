using FlexFramework.Core;
using FlexFramework.Core.Rendering;

namespace FlexFramework.Core;

public interface IRenderable
{
    void Render(RenderArgs args);
}