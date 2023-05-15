namespace FlexFramework.Core.UserInterface.Elements;

public class EmptyElement : Element
{
    public EmptyElement(params Element[] children)
    {
        Children.AddRange(children);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
    }
}