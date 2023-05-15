using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core;

public class Input
{
    private readonly NativeWindow window;
    private readonly KeyboardState keyboard;
    private readonly MouseState mouse;

    public bool WindowFocused => window.IsFocused;
    public Vector2 MousePosition => mouse.Position;
    public Vector2 MouseDelta => mouse.Delta;
    public Vector2 MouseScroll => mouse.Scroll;
    public Vector2 MouseScrollDelta => mouse.ScrollDelta;

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

    public Vector2 GetMovement()
    {
        float x = (GetKey(Keys.D) || GetKey(Keys.Right) ? 1.0f : 0.0f) + (GetKey(Keys.A) || GetKey(Keys.Left) ? -1.0f : 0.0f);
        float y = (GetKey(Keys.W) || GetKey(Keys.Up) ? 1.0f : 0.0f) + (GetKey(Keys.S) || GetKey(Keys.Down) ? -1.0f : 0.0f);
        return new Vector2(x, y);
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