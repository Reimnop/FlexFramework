using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public abstract class UIElement : Entity
{
    public abstract Vector2 Position { get; set; }
    public abstract Vector2 Size { get; set; }
    public abstract Vector2 Origin { get; set; }
    public abstract bool IsFocused { get; set; }

    private bool wasFocused = false;

    protected FlexFrameworkMain Engine { get; }

    public UIElement(FlexFrameworkMain engine)
    {
        Engine = engine;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        bool currentlyFocused = IsFocused;

        if (!wasFocused && currentlyFocused)
        {
            OnFocused();
        }
        else if (wasFocused && !currentlyFocused)
        {
            OnUnfocused();
        }
        
        wasFocused = currentlyFocused;
    }

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = Engine.Input.MousePosition;
        return new Vector2(mousePos.X, Engine.ClientSize.Y - mousePos.Y);
    }

    public bool IsMouseOver()
    {
        Vector2 mousePos = GetMousePos();
        Bounds bounds = GetBounds();
        return bounds.Contains(mousePos.X, mousePos.Y);
    }

    public Bounds GetBounds()
    {
        return new Bounds(
            Position.X - Origin.X * Size.X, 
            Position.Y - Origin.Y * Size.Y, 
            Position.X + (1.0f - Origin.X) * Size.X, 
            Position.Y + (1.0f - Origin.Y) * Size.Y);
    }

    protected virtual void OnUnfocused()
    {
    }

    protected virtual void OnFocused()
    {
    }
}