using MsdfGenNet.Native;

namespace MsdfGenNet;

public class Shape : NativeObject
{
    public Shape() : base(MsdfGenNative.msdfgen_Shape_new())
    {
    }
    
    internal Shape(IntPtr handle) : base(handle)
    {
    }

    public void AddContour(Contour contour)
    {
        MsdfGenNative.msdfgen_Shape_addContour(Handle, contour.Handle);
    }

    public Contour AddContour()
    {
        IntPtr contourHandle = MsdfGenNative.msdfgen_Shape_addContourNew(Handle);
        return new Contour(contourHandle);
    }
    
    public void Normalize()
    {
        MsdfGenNative.msdfgen_Shape_normalize(Handle);
    }
    
    public bool Validate()
    {
        return MsdfGenNative.msdfgen_Shape_validate(Handle);
    }
    
    public void Bound(ref double l, ref double b, ref double r, ref double t)
    {
        MsdfGenNative.msdfgen_Shape_bound(Handle, ref l, ref b, ref r, ref t);
    }
    
    public void BoundMiters(ref double l, ref double b, ref double r, ref double t, double border, double miterLimit, int polarity)
    {
        MsdfGenNative.msdfgen_Shape_boundMiters(Handle, ref l, ref b, ref r, ref t, border, miterLimit, polarity);
    }

    public Bounds GetBounds()
    {
        return MsdfGenNative.msdfgen_Shape_getBounds(Handle);
    }

    // TODO: Implement msdfgen::Scanline
    // public void Scanline(Scanline line, double y)
    // {
    //     MsdfGenNative.msdfgen_Shape_scanline(Handle, line.Handle, y);
    // }
    
    public int EdgeCount()
    {
        return MsdfGenNative.msdfgen_Shape_edgeCount(Handle);
    }
    
    public void OrientContours()
    {
        MsdfGenNative.msdfgen_Shape_orientContours(Handle);
    }

    protected override void FreeHandle()
    {
        MsdfGenNative.msdfgen_Shape_free(Handle);
    }
}