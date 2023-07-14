using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core;

/// <summary>
/// Provides a mechanism for receiving input from the user
/// </summary>
public class Input
{
    private readonly NativeWindow window;
    private readonly KeyboardState keyboard;
    private readonly MouseState mouse;

    /// <summary>
    /// Is the window focused?
    /// </summary>
    public bool WindowFocused => window.IsFocused;
    
    /// <summary>
    /// The position of the cursor in screen space
    /// </summary>
    public Vector2 MousePosition => mouse.Position;
    
    /// <summary>
    /// The difference in position of the cursor in screen space since the last frame
    /// </summary>
    public Vector2 MouseDelta => mouse.Delta;
    
    /// <summary>
    /// The position of the mouse scroll wheel
    /// </summary>
    public Vector2 MouseScroll => mouse.Scroll;
    
    /// <summary>
    /// The difference in position of the mouse scroll wheel since the last frame
    /// </summary>
    public Vector2 MouseScrollDelta => mouse.ScrollDelta;

    /// <summary>
    /// Is any key down?
    /// </summary>
    public bool AnyKeyDown => IsInputAvailable() && keyboard.IsAnyKeyDown;
    
    /// <summary>
    /// Is any mouse button down?
    /// </summary>
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

    
    /// <summary>
    /// Get if the user clicked the mouse button
    /// </summary>
    /// <param name="button">The mouse button</param>
    /// <returns>true if the mouse button was clicked</returns>
    public bool GetMouseDown(MouseButton button)
    {
        if (!IsInputAvailable())
        {
            return false;
        }

        return !mouse.WasButtonDown(button) && mouse.IsButtonDown(button);
    }

    /// <summary>
    /// Get if the user released the mouse button
    /// </summary>
    /// <param name="button">The mouse button</param>
    /// <returns>true if the mouse button was released</returns>
    public bool GetMouseUp(MouseButton button)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return !mouse.IsButtonDown(button) && mouse.WasButtonDown(button);
    }
    
    /// <summary>
    /// Get if the mouse button is held down
    /// </summary>
    /// <param name="button">The mouse button</param>
    /// <returns>true if the mouse button is held down</returns>
    public bool GetMouse(MouseButton button)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return mouse.IsButtonDown(button);
    }

    /// <summary>
    /// Get if the user pressed the key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>true if the key was pressed</returns>
    public bool GetKeyDown(Keys key)
    {
        if (!IsInputAvailable())
        {
            return false;
        }

        return !keyboard.WasKeyDown(key) && keyboard.IsKeyDown(key);
    }
    
    /// <summary>
    /// Get if the user released the key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>true if the key was released</returns>
    public bool GetKeyUp(Keys key)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return !keyboard.IsKeyDown(key) && keyboard.WasKeyDown(key);
    }

    /// <summary>
    /// Get if the user held down the key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>true if the key was held down</returns>
    public bool GetKey(Keys key)
    {
        if (!IsInputAvailable())
        {
            return false;
        }
        
        return keyboard.IsKeyDown(key);
    }

    /// <summary>
    /// Get the WASD or arrow key movement vector
    /// </summary>
    /// <returns>Movement vector in range [-1.0; 1.0]</returns>
    public Vector2 GetMovement()
    {
        float x = (GetKey(Keys.D) || GetKey(Keys.Right) ? 1.0f : 0.0f) + (GetKey(Keys.A) || GetKey(Keys.Left) ? -1.0f : 0.0f);
        float y = (GetKey(Keys.W) || GetKey(Keys.Up) ? 1.0f : 0.0f) + (GetKey(Keys.S) || GetKey(Keys.Down) ? -1.0f : 0.0f);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Get if a key combination was pressed
    /// </summary>
    /// <param name="keys">Array of keys to check for</param>
    /// <returns>true if the key combination was pressed</returns>
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