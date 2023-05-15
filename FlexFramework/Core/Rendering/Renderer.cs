using FlexFramework.Core.Rendering.BackgroundRenderers;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Core;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public abstract class Renderer
{
    public abstract GpuInfo GpuInfo { get; }
    public Color4 ClearColor { get; set; } = Color4.Black;

    protected FlexFrameworkMain Engine { get; private set; } = null!;

    internal void SetEngine(FlexFrameworkMain engine)
    {
        Engine = engine;
    }

    public abstract void Init();
    public abstract IRenderBuffer CreateRenderBuffer(Vector2i size);
    public abstract void Update(UpdateArgs args);
    public abstract void Render(Vector2i size, CommandList commandList, IRenderBuffer renderBuffer);
}