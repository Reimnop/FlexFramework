namespace MsdfGenNet.Native;

public abstract class NativeObject : IDisposable
{
    internal IntPtr Handle { get; private set; }

    protected NativeObject(IntPtr handle)
    {
        Handle = handle;
    }

    ~NativeObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Handle != IntPtr.Zero)
        {
            FreeHandle();
            Handle = IntPtr.Zero;
        }
    }

    protected abstract void FreeHandle();
}