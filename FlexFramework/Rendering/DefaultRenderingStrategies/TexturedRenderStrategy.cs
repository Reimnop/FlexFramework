using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering.DefaultRenderingStrategies;

public class TexturedRenderStrategy : RenderingStrategy
{
    private readonly ShaderProgram unlitShader;

    public TexturedRenderStrategy(ShaderProgram unlitShader)
    {
        this.unlitShader = unlitShader;
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        TexturedVertexDrawData texturedVertexDrawData = EnsureDrawDataType<TexturedVertexDrawData>(drawData);
        
        glStateManager.UseProgram(unlitShader.Handle);
        glStateManager.BindVertexArray(texturedVertexDrawData.VertexArray.Handle);

        Matrix4 transformation = texturedVertexDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.Uniform1(1, 1);
        GL.Uniform1(2, 0);
        glStateManager.BindTextureUnit(0, texturedVertexDrawData.Texture.Handle);
                
        GL.Uniform4(3, texturedVertexDrawData.Color);
                
        GL.DrawArrays(PrimitiveType.Triangles, 0, texturedVertexDrawData.Count);
    }

    public override void Dispose()
    {
    }
}