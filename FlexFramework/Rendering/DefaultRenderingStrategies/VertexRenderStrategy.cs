using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering.DefaultRenderingStrategies;

public class VertexRenderStrategy : RenderingStrategy
{
    private readonly ShaderProgram unlitShader;

    public VertexRenderStrategy(ShaderProgram unlitShader)
    {
        this.unlitShader = unlitShader;
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        VertexDrawData vertexDrawData = EnsureDrawDataType<VertexDrawData>(drawData);
        
        glStateManager.UseProgram(unlitShader.Handle);
        glStateManager.BindVertexArray(vertexDrawData.VertexArray.Handle);

        Matrix4 transformation = vertexDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.Uniform1(1, 0);
        GL.Uniform4(3, vertexDrawData.Color);

        GL.DrawArrays(PrimitiveType.Triangles, 0, vertexDrawData.Count);
    }

    public override void Dispose()
    {
    }
}