using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering;

public class GLStateManager
{
    private int currentProgram = 0;
    private int currentVertexArray = 0;
    private int[] currentTextureUnits = new int[16];

    public void UseProgram(int program)
    {
        if (currentProgram == program)
        {
            return;
        }
        
        currentProgram = program;
        GL.UseProgram(program);
    }
    
    public void BindVertexArray(int vertexArray)
    {
        if (currentVertexArray == vertexArray)
        {
            return;
        }
        
        currentVertexArray = vertexArray;
        GL.BindVertexArray(vertexArray);
    }
    
    public void BindTextureUnit(int unit, int texture)
    {
        if (currentTextureUnits[unit] == texture)
        {
            return;
        }
        
        currentTextureUnits[unit] = texture;
        GL.BindTextureUnit(unit, texture);
    }
}