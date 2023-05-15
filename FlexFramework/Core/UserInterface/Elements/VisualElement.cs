using System.Diagnostics;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class VisualElement : Element, IRenderable
{
    public Transform RenderTransform { get; set; } = new Transform();

    protected VisualElement(params Element[] elements) : base(elements)
    {
    }

    public abstract void Render(RenderArgs args);
}