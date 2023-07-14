using FlexFramework.Core.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct RenderBufferDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public IRenderBuffer RenderBuffer { get; }
    public PrimitiveType PrimitiveType { get; }

    public RenderBufferDrawData(IMeshView mesh, Matrix4 transformation, IRenderBuffer renderBuffer, PrimitiveType primitiveType)
    {
        Mesh = mesh;
        Transformation = transformation;
        RenderBuffer = renderBuffer;
        PrimitiveType = primitiveType;
    }
}