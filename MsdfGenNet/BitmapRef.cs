using System.Runtime.InteropServices;

namespace MsdfGenNet;

internal struct BitmapRef
{
    private IntPtr data;
    private int width, height;

    internal BitmapRef(IntPtr data, int width, int height)
    {
        this.data = data;
        this.width = width;
        this.height = height;
    }
}