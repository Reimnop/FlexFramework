using MsdfGenNet.Native;

namespace MsdfGenNet;

// Wrapper for EdgeHolder, but simplified to just Edge
public class Edge : NativeObject
{
    public Edge() : base(MsdfGenNative.msdfgen_EdgeHolder_new())
    {
    }
    
    public Edge(Vector2d p0, Vector2d p1, EdgeColor edgeColor = EdgeColor.White) : base(MsdfGenNative.msdfgen_EdgeHolder_newLinear(p0, p1, edgeColor))
    {
    }
    
    public Edge(Vector2d p0, Vector2d p1, Vector2d p2, EdgeColor edgeColor = EdgeColor.White) : base(MsdfGenNative.msdfgen_EdgeHolder_newQuadratic(p0, p1, p2, edgeColor))
    {
    }
    
    public Edge(Vector2d p0, Vector2d p1, Vector2d p2, Vector2d p3, EdgeColor edgeColor = EdgeColor.White) : base(MsdfGenNative.msdfgen_EdgeHolder_newCubic(p0, p1, p2, p3, edgeColor))
    {
    }

    public Edge(Edge edge) : base(MsdfGenNative.msdfgen_EdgeHolder_newClone(edge.Handle))
    {
    }
    
    internal Edge(IntPtr handle) : base(handle)
    {
    }

    protected override void FreeHandle()
    {
        MsdfGenNative.msdfgen_EdgeHolder_free(Handle);
    }
}