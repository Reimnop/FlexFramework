using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core;

namespace FlexFramework.Core.Rendering.BackgroundRenderers;

public abstract class BackgroundRenderer
{
    public abstract void Render(CommandList commandList, GLStateManager stateManager, IRenderBuffer renderBuffer, CameraData cameraData);
}