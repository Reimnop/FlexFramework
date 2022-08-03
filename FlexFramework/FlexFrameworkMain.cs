using System.ComponentModel;
using System.Runtime.InteropServices;
using FlexFramework.Audio;
using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Text;
using FlexFramework.Util.Exceptions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework;

public class FlexFrameworkMain : NativeWindow
{
    public Renderer Renderer { get; private set; }
    public TextResources TextResources { get; private set; }
    public PersistentResources PersistentResources { get; }
    public SceneManager SceneManager { get; }
    public AudioManager AudioManager { get; }

    private double time = 0.0;

    private GCHandle leakedGcHandle;

    public FlexFrameworkMain(NativeWindowSettings nws) : base(nws)
    {
        Context.MakeCurrent();
        
        // init GL debug callback
        GL.Enable(EnableCap.DebugOutput);
#if DEBUG
        GL.Enable(EnableCap.DebugOutputSynchronous);
#endif

        DebugProc debugProc = LogGlMessage;
        leakedGcHandle = GCHandle.Alloc(debugProc);
        GL.DebugMessageCallback(debugProc, IntPtr.Zero);

        SceneManager = new SceneManager(this);
        PersistentResources = new PersistentResources();
        AudioManager = new AudioManager();
    }

    private void LogGlMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        if (severity == DebugSeverity.DebugSeverityNotification)
        {
            return;
        }
        
        string messageString = Marshal.PtrToStringAnsi(message, length);

        Console.WriteLine($"{severity} {type} | {messageString}");

        if (type == DebugType.DebugTypeError)
        {
            throw new Exception(messageString);
        }
    }
    
    public T? UseRenderer<T>(params object?[]? args) where T : Renderer
    {
        T? renderer = (T?) Activator.CreateInstance(typeof(T), args);

        if (renderer == null)
        {
            return null;
        }

        if (Renderer != null)
        {
            Renderer.Dispose();
        }

        renderer.SetEngine(this);
        renderer.Init();

        Renderer = renderer;
        return renderer;
    }

    public TextResources LoadFonts(int size, int atlasWidth, params FontFileInfo[] fontFiles)
    {
        if (TextResources != null)
        {
            TextResources.Dispose();
        }

        TextResources = new TextResources(size, atlasWidth, fontFiles);
        return TextResources;
    }

    public T LoadScene<T>(params object?[]? args) where T : Scene
    {
        return SceneManager.LoadScene<T>(args);
    }

    public void Update()
    {
        GLFW.PollEvents();

        double currentTime = GLFW.GetTime();
        double deltaTime = currentTime - time;
        time = currentTime;

        UpdateScene(deltaTime);
        Render();
    }

    private void UpdateScene(double deltaTime)
    {
        if (SceneManager.CurrentScene == null)
        {
            throw new NoSceneException();
        }

        UpdateArgs args = new UpdateArgs(time, deltaTime);
        
        SceneManager.CurrentScene.UpdateInternal(args);
        SceneManager.CurrentScene.Update(args);
    }

    private unsafe void Render()
    {
        if (Renderer == null)
        {
            throw new NoRendererException();
        }
        
        SceneManager.CurrentScene.Render(Renderer);
        Renderer.Render();
        
        GLFW.SwapBuffers(WindowPtr);
    }

    public unsafe bool ShouldClose()
    {
        return GLFW.WindowShouldClose(WindowPtr);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        
        leakedGcHandle.Free();
    }
}