using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core;

namespace FlexFramework.Core.Rendering.BackgroundRenderers;

public abstract class BackgroundRenderer
{
    public abstract void Render(Renderer renderer, GLStateManager stateManager, Texture2D renderTarget, CameraData cameraData);
}