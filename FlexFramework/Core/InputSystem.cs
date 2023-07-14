using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core;

public struct InputCapture
{
    public int Id { get; }

    public InputCapture(int id)
    {
        Id = id;
    }
}

public class InputSystem
{
    public Input Input { get; }
    
    private readonly List<InputCapture> captures = new List<InputCapture>();
    private InputCapture? currentCapture = null;
    private int captureId = 0;

    public InputSystem(Input input)
    {
        Input = input;
    }

    public void Update()
    {
        currentCapture = captures.Count > 0 ? captures[^1] : null;
    }

    public ScopedInputProvider AcquireInputProvider()
    {
        return new ScopedInputProvider(AcquireCapture(), this);
    }

    public InputCapture AcquireCapture()
    {
        InputCapture capture = new InputCapture(captureId++);
        captures.Add(capture);
        return capture;
    }

    public bool IsCurrentCapture(InputCapture capture)
    {
        if (captures.Count == 0)
        {
            return false;
        }
        
        if (currentCapture == null)
        {
            return false;
        }
        
        return currentCapture.Value.Id == capture.Id;
    }

    public void ReleaseCapture(InputCapture capture)
    {
        captures.Remove(capture);
    }
    
    public Vector2 GetMousePosition(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }
        
        return Input.MousePosition;
    }
    
    public Vector2 GetMouseDelta(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }
        
        return Input.MouseDelta;
    }
    
    public Vector2 GetMouseScroll(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }
        
        return Input.MouseScroll;
    }
    
    public Vector2 GetMouseScrollDelta(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }
        
        return Input.MouseScrollDelta;
    }
    
    public bool GetMouseDown(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetMouseDown(button);
    }

    public bool GetMouseUp(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetMouseUp(button);
    }
    
    public bool GetMouse(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetMouse(button);
    }

    public bool GetKeyDown(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKeyDown(key);
    }
    
    public bool GetKeyUp(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKeyUp(key);
    }

    public bool GetKey(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKey(key);
    }

    public Vector2 GetMovement(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }

        return Input.GetMovement();
    }

    public bool GetKeyCombo(InputCapture capture, params Keys[] keys)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKeyCombo(keys);
    }
}