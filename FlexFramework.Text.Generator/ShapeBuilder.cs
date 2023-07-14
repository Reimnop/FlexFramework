using System.Diagnostics;
using MsdfGenNet;
using SharpFont;

namespace FlexFramework.Text.Generator;

public class ShapeBuilder
{
    public Shape Shape => shape;
    
    private readonly Shape shape;
    private Contour? currentContour;
    private Vector2d lastPoint;

    public ShapeBuilder(Outline outline)
    {
        shape = new Shape();
        
        OutlineFuncs funcs = new OutlineFuncs();
        funcs.MoveFunction = MoveTo;
        funcs.LineFunction = LineTo;
        funcs.ConicFunction = ConicTo;
        funcs.CubicFunction = CubicTo;
        funcs.Shift = 0;

        outline.Decompose(funcs, IntPtr.Zero);

        if (currentContour != null)
        {
            shape.AddContour(currentContour);
        }
    }

    private Vector2d FromFtVector(ref FTVector vector)
    {
        return new Vector2d(vector.X.Value / 64.0, vector.Y.Value / 64.0);
    }
    
    private int MoveTo(ref FTVector to, IntPtr context)
    {
        if (currentContour != null)
        {
            shape.AddContour(currentContour);
        }
        
        currentContour = new Contour();
        lastPoint = FromFtVector(ref to);
        return 0;
    }

    private int LineTo(ref FTVector to, IntPtr context)
    {
        Debug.Assert(currentContour != null);
        currentContour.AddEdge(new Edge(lastPoint, FromFtVector(ref to)));
        lastPoint = FromFtVector(ref to);
        return 0;
    }

    private int ConicTo(ref FTVector control, ref FTVector to, IntPtr context)
    {
        Debug.Assert(currentContour != null);
        currentContour.AddEdge(new Edge(lastPoint, FromFtVector(ref control), FromFtVector(ref to)));
        lastPoint = FromFtVector(ref to);
        return 0;
    }

    private int CubicTo(ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr context)
    {
        Debug.Assert(currentContour != null);
        currentContour.AddEdge(new Edge(lastPoint, FromFtVector(ref control1), FromFtVector(ref control2),FromFtVector(ref to)));
        lastPoint = FromFtVector(ref to);
        return 0;
    }
}