namespace FlexFramework.Core.UserInterface.Elements;

public enum Direction
{
    Horizontal,
    Vertical
}

public class StackLayoutElement : Element
{
    public Length Spacing { get; set; } = Length.Zero;
    public Direction Direction { get; set; } = Direction.Vertical;

    public StackLayoutElement(Direction direction = Direction.Vertical, params Element[] children)
    {
        Direction = direction;
        Children.AddRange(children);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);

        float spacing = Spacing.Calculate(ContentBounds.Height);

        // Create child drawables
        if (Direction == Direction.Vertical)
        {
            float y = ContentBounds.Y0;
            foreach (Element child in Children)
            {
                // The child bounds are constrained to the parent bounds
                Bounds childConstraintBounds = new Bounds(ContentBounds.X0, y, ContentBounds.X1, ContentBounds.Y1 + y);

                // Calculate the child bounds
                Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
                y += childBounds.Height + spacing; // Add the spacing to the y position

                // Add the child drawables
                child.UpdateLayout(childConstraintBounds);
            }
        }
        else
        {
            float x = ContentBounds.X0;
            foreach (Element child in Children)
            {
                // The child bounds are constrained to the parent bounds
                Bounds childConstraintBounds = new Bounds(x, ContentBounds.Y0, ContentBounds.X1 + x, ContentBounds.Y1);

                // Calculate the child bounds
                Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
                x += childBounds.Width + spacing; // Add the spacing to the x position

                // Add the child drawables
                child.UpdateLayout(childConstraintBounds);
            }
        }
    }
}