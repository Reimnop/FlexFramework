namespace FlexFramework.Core.UserInterface.Elements;

public abstract class VisualElement : Element, IRenderable
{
    public abstract void Render(RenderArgs args);
}