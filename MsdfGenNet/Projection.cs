using MsdfGenNet.Native;

namespace MsdfGenNet;

public class Projection : NativeObject
{
    public Projection() : base(MsdfGenNative.msdfgen_Projection_new())
    {
    }

    public Projection(ref Vector2d scale, ref Vector2d translate) : base(MsdfGenNative.msdfgen_Projection_newScaleTranslate(ref scale, ref translate))
    {
    }
    
    internal Projection(IntPtr handle) : base(handle)
    {
    }

    public Vector2d Project(ref Vector2d coord)
    {
        return MsdfGenNative.msdfgen_Projection_project(Handle, ref coord);
    }
    
    public Vector2d Unproject(ref Vector2d coord)
    {
        return MsdfGenNative.msdfgen_Projection_unproject(Handle, ref coord);
    }
    
    public double ProjectX(double x)
    {
        return MsdfGenNative.msdfgen_Projection_projectX(Handle, x);
    }
    
    public double ProjectY(double y)
    {
        return MsdfGenNative.msdfgen_Projection_projectY(Handle, y);
    }
    
    public double UnprojectX(double x)
    {
        return MsdfGenNative.msdfgen_Projection_unprojectX(Handle, x);
    }
    
    public double UnprojectY(double y)
    {
        return MsdfGenNative.msdfgen_Projection_unprojectY(Handle, y);
    }

    protected override void FreeHandle()
    {
        MsdfGenNative.msdfgen_Projection_free(Handle);
    }
}