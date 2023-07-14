using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public delegate void RenderCallback(Vector2i viewportSize, CommandList commandList);

public class ViewportElement : VisualElement, IRenderable, IDisposable
{
    private readonly Renderer renderer;
    private readonly CommandList commandList = new();
    
    private readonly IRenderBuffer renderBuffer;
    private readonly RenderCallback renderCallback;

    private Box2 bounds;

    public ViewportElement(Renderer renderer, RenderCallback renderCallback)
    {
        this.renderer = renderer;
        this.renderCallback = renderCallback;
        renderBuffer = renderer.CreateRenderBuffer(Vector2i.One);
    }
    
    protected override void UpdateLayout(Box2 bounds)
    {
        this.bounds = bounds;
    }

    public override void Render(RenderArgs args)
    {
        var viewportSize = (Vector2i) bounds.Size;
        
        commandList.Clear();
        renderCallback(viewportSize, commandList);
        renderer.Render(viewportSize, commandList, renderBuffer);
        
        var matrixStack = args.MatrixStack;
        matrixStack.Push();
        matrixStack.Scale(1.0f, -1.0f, 0.0f);
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(bounds.Size.X, bounds.Size.Y, 1.0f);
        matrixStack.Translate(bounds.Min.X, bounds.Min.Y, 0.0f);
        
        var transformation = matrixStack.GlobalTransformation * args.CameraData.View * args.CameraData.Projection;
        var drawData = new RenderBufferDrawData(DefaultAssets.QuadMesh.ReadOnly, transformation, renderBuffer, PrimitiveType.Triangles);
        args.CommandList.AddDrawData(LayerType.Gui, drawData);
        
        matrixStack.Pop();
    }

    public void Dispose()
    {
        if (renderBuffer is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}