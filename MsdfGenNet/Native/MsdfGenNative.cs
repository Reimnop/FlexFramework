using System.Runtime.InteropServices;

using static MsdfGenNet.Native.Constants;

namespace MsdfGenNet.Native;

// msdfgen_wrap.h
// This is a wrapper around the msdfgen library, which is a C++ library.
internal static class MsdfGenNative
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generateSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generatePseudoSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generateMSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generateMTSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_edgeColoringSimple(IntPtr shape, double angleThreshold, ulong seed = 0);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_edgeColoringInkTrap(IntPtr shape, double angleThreshold, ulong seed = 0);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_edgeColoringByDistance(IntPtr shape, double angleThreshold, ulong seed = 0);
    
    // Shape
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Shape_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_free(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_addContour(IntPtr shape, IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Shape_addContourNew(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_normalize(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool msdfgen_Shape_validate(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_bound(IntPtr shape, ref double l, ref double b, ref double r, ref double t);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_boundMiters(IntPtr shape, ref double l, ref double b, ref double r, ref double t, double border, double miterLimit, int polarity);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Bounds msdfgen_Shape_getBounds(IntPtr shape);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_scanline(IntPtr shape, IntPtr line, double y);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int msdfgen_Shape_edgeCount(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_orientContours(IntPtr shape);
    
    // Contour
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Contour_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_free(IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_addEdge(IntPtr contour, IntPtr edge);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Contour_addEdgeNew(IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_bound(IntPtr contour, ref double l, ref double b, ref double r, ref double t);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_boundMiters(IntPtr contour, ref double l, ref double b, ref double r, ref double t, double border, double miterLimit, int polarity);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int msdfgen_Contour_winding(IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_reverse(IntPtr contour);
    
    // EdgeHolder
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newFromEdgeSegment(IntPtr edgeSegment); // hell no
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newLinear(Vector2d p0, Vector2d p1, EdgeColor edgeColor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newQuadratic(Vector2d p0, Vector2d p1, Vector2d p2, EdgeColor edgeColor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newCubic(Vector2d p0, Vector2d p1, Vector2d p2, Vector2d p3, EdgeColor edgeColor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newClone(IntPtr edge);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_EdgeHolder_free(IntPtr edge);
    
    // Projection
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Projection_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Projection_newScaleTranslate(ref Vector2d scale, ref Vector2d translate);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Projection_free(IntPtr projection);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Vector2d msdfgen_Projection_project(IntPtr projection, ref Vector2d coord);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Vector2d msdfgen_Projection_unproject(IntPtr projection, ref Vector2d coord);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double msdfgen_Projection_projectX(IntPtr projection, double x);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double msdfgen_Projection_projectY(IntPtr projection, double y);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double msdfgen_Projection_unprojectX(IntPtr projection, double x);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double msdfgen_Projection_unprojectY(IntPtr projection, double y);
}