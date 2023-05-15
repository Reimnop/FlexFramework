using MsdfGenNet.Native;

namespace MsdfGenNet;

public class Contour : NativeObject
{
    public Contour() : base(MsdfGenNative.msdfgen_Contour_new())
    {
    }
    
    internal Contour(IntPtr handle) : base(handle)
    {
    }

    public void AddEdge(Edge edge)
    {
        MsdfGenNative.msdfgen_Contour_addEdge(Handle, edge.Handle);
    }

    public Edge AddEdge()
    {
        IntPtr edgeHandle = MsdfGenNative.msdfgen_Contour_addEdgeNew(Handle);
        return new Edge(edgeHandle);
    }
    
    public void Bound(ref double l, ref double b, ref double r, ref double t)
    {
        MsdfGenNative.msdfgen_Contour_bound(Handle, ref l, ref b, ref r, ref t);
    }
    
    public void BoundMiters(ref double l, ref double b, ref double r, ref double t, double border, double miterLimit, int polarity)
    {
        MsdfGenNative.msdfgen_Contour_boundMiters(Handle, ref l, ref b, ref r, ref t, border, miterLimit, polarity);
    }

    public int Winding()
    {
        return MsdfGenNative.msdfgen_Contour_winding(Handle);
    }
    
    public void Reverse()
    {
        MsdfGenNative.msdfgen_Contour_reverse(Handle);
    }

    protected override void FreeHandle()
    {
        MsdfGenNative.msdfgen_Contour_free(Handle);
    }
}