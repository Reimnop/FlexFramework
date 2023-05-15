using MsdfGenNet.Native;

namespace MsdfGenNet;

public static class MsdfGen
{
    public static unsafe void GenerateSDF(Bitmap<float> output, Shape shape, Projection projection, double range, ref GeneratorConfig config)
    {
        if (output.NumChannels != 1)
        {
            throw new ArgumentException("Output image must have 1 channel!");
        }
        
        BitmapRef bitmapRef = output.Reference;
        IntPtr shapeHandle = shape.Handle;
        IntPtr projectionHandle = projection.Handle;
        
        fixed (GeneratorConfig* configPtr = &config)
        {
            IntPtr bitmapRefPtr = new IntPtr(&bitmapRef);
            IntPtr configPtrPtr = new IntPtr(configPtr);
            MsdfGenNative.msdfgen_generateSDF(bitmapRefPtr, shapeHandle, projectionHandle, range, configPtrPtr);
        }
    }
    
    public static unsafe void GeneratePseudoSDF(Bitmap<float> output, Shape shape, Projection projection, double range, ref GeneratorConfig config)
    {
        if (output.NumChannels != 1)
        {
            throw new ArgumentException("Output image must have 1 channel!");
        }
        
        BitmapRef bitmapRef = output.Reference;
        IntPtr shapeHandle = shape.Handle;
        IntPtr projectionHandle = projection.Handle;
        
        fixed (GeneratorConfig* configPtr = &config)
        {
            IntPtr bitmapRefPtr = new IntPtr(&bitmapRef);
            IntPtr configPtrPtr = new IntPtr(configPtr);
            MsdfGenNative.msdfgen_generatePseudoSDF(bitmapRefPtr, shapeHandle, projectionHandle, range, configPtrPtr);
        }
    }
    
    public static unsafe void GenerateMSDF(Bitmap<float> output, Shape shape, Projection projection, double range, ref MSDFGeneratorConfig config)
    {
        if (output.NumChannels != 3)
        {
            throw new ArgumentException("Output image must have 3 channels!");
        }
        
        BitmapRef bitmapRef = output.Reference;
        IntPtr shapeHandle = shape.Handle;
        IntPtr projectionHandle = projection.Handle;
        
        fixed (MSDFGeneratorConfig* configPtr = &config)
        {
            IntPtr bitmapRefPtr = new IntPtr(&bitmapRef);
            IntPtr configPtrPtr = new IntPtr(configPtr);
            MsdfGenNative.msdfgen_generateMSDF(bitmapRefPtr, shapeHandle, projectionHandle, range, configPtrPtr);
        }
    }
    
    public static unsafe void GenerateMTSDF(Bitmap<float> output, Shape shape, Projection projection, double range, ref MSDFGeneratorConfig config)
    {
        if (output.NumChannels != 4)
        {
            throw new ArgumentException("Output image must have 4 channels!");
        }
        
        BitmapRef bitmapRef = output.Reference;
        IntPtr shapeHandle = shape.Handle;
        IntPtr projectionHandle = projection.Handle;
        
        fixed (MSDFGeneratorConfig* configPtr = &config)
        {
            IntPtr bitmapRefPtr = new IntPtr(&bitmapRef);
            IntPtr configPtrPtr = new IntPtr(configPtr);
            MsdfGenNative.msdfgen_generateMTSDF(bitmapRefPtr, shapeHandle, projectionHandle, range, configPtrPtr);
        }
    }
    
    public static void EdgeColoringSimple(Shape shape, double angleThreshold, ulong seed = 0)
    {
        IntPtr shapeHandle = shape.Handle;
        MsdfGenNative.msdfgen_edgeColoringSimple(shapeHandle, angleThreshold, seed);
    }
    
    public static void EdgeColoringInkTrap(Shape shape, double angleThreshold, ulong seed = 0)
    {
        IntPtr shapeHandle = shape.Handle;
        MsdfGenNative.msdfgen_edgeColoringInkTrap(shapeHandle, angleThreshold, seed);
    }
    
    public static void EdgeColoringByDistance(Shape shape, double angleThreshold, ulong seed = 0)
    {
        IntPtr shapeHandle = shape.Handle;
        MsdfGenNative.msdfgen_edgeColoringByDistance(shapeHandle, angleThreshold, seed);
    }
}