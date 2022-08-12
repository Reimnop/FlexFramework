using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core;

public class Input
{
    private readonly NativeWindow window;
    private readonly KeyboardState keyboard;
    private readonly MouseState mouse;

    public Vector2d MousePosition => mouse.Position;
    public Vector2d MouseDelta => mouse.Delta;
    public Vector2d MouseScroll => mouse.Scroll;
    public Vector2d MouseScrollDelta => mouse.ScrollDelta;

    public bool AnyKeyDown => IsInputAvailable() && keyboard.IsAnyKeyDown;
    public bool AnyMouseButtonDown => IsInputAvailable() && mouse.IsAnyButtonDown;
        
    public Input(NativeWindow window)
    {
        this.window = window;
        keyboard = window.KeyboardState;
        mouse = window.MouseState;
    }

    private bool IsInputAvailable()
    {
        return window.IsFocused;
    }

    public bool GetMouseDown(MouseButton button)
    {
        if (!IsInputAvailable())
        {
            return false;
        }

        return !mouse.WasButtonDown(button) && mouse.IsButtonDown(button);
    }

    public bool GetMouseUp(MouseButton button)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return !mouse.IsButtonDown(button) && mouse.WasButtonDown(button);
    }
    
    public bool GetMouse(MouseButton button)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return mouse.IsButtonDown(button);
    }

    public bool GetKeyDown(Keys key)
    {
        if (!IsInputAvailable())
        {
            return false;
        }

        return !keyboard.WasKeyDown(key) && keyboard.IsKeyDown(key);
    }
    
    public bool GetKeyUp(Keys key)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return !keyboard.IsKeyDown(key) && keyboard.WasKeyDown(key);
    }

    public bool GetKey(Keys key)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return keyboard.IsKeyDown(key);
    }

    public bool GetKeyCombo(params Keys[] keys)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        bool allPreviousKeysDown = true;
        for (int i = 0; i < keys.Length - 1; i++)
        {
            allPreviousKeysDown &= GetKey(keys[i]);
        }

        return allPreviousKeysDown && GetKeyDown(keys[^1]);
    }
}