using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface;

public delegate void MouseEventHandler(MouseButton button);

public class Interactivity : IUpdateable // In case you want to cast it to IUpdateable (don't do that)
{
    public event Action? MouseEnter;
    public event Action? MouseLeave;
    public event MouseEventHandler? MouseButtonDown;
    public event MouseEventHandler? MouseButtonUp;
    
    public Box2 Bounds { get; set; }
    public bool MouseOver { get; private set; }
    public bool[] MouseButtons { get; } = new bool[(int) MouseButton.Last + 1];
    
    private readonly IInputProvider inputProvider;
    private bool lastMouseOver;

    public Interactivity(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }

    public void Update()
    {
        // Get input
        MouseOver = IsMouseOver();

        // Check for mouse enter/leave
        if (MouseOver && !lastMouseOver)
        {
            MouseEnter?.Invoke();
        }
        else if (!MouseOver && lastMouseOver)
        {
            MouseLeave?.Invoke();
        }
        
        // Update last values
        lastMouseOver = MouseOver;

        for (int i = 0; i < MouseButtons.Length; i++)
        {
            MouseButtons[i] = inputProvider.GetMouse((MouseButton) i) && MouseOver;
            
            if (inputProvider.GetMouseDown((MouseButton) i) && MouseOver)
            {
                MouseButtonDown?.Invoke((MouseButton) i);
            }
            else if (inputProvider.GetMouseUp((MouseButton) i) && MouseOver)
            {
                MouseButtonUp?.Invoke((MouseButton) i);
            }
        }
    }
    
    public void Update(UpdateArgs args)
    {
        Update();
    }
    
    private bool IsMouseOver()
    {
        return inputProvider.InputAvailable && Bounds.ContainsInclusive(inputProvider.MousePosition);
    }
    
    private bool IsMouseButton(MouseButton button)
    {
        return inputProvider.InputAvailable && inputProvider.GetMouse(button);
    }
}