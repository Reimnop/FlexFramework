using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering.DefaultRenderingStrategies;

public class TextRenderStrategy : RenderingStrategy
{
    private readonly ShaderProgram textShader;
    private readonly FlexFrameworkMain engine;

    public TextRenderStrategy(FlexFrameworkMain engine)
    {
        this.engine = engine;

        using Shader textVert = new Shader("text-vert", File.ReadAllText("Assets/Shaders/text.vert"),
            ShaderType.VertexShader);
        using Shader textFrag = new Shader("text-frag", File.ReadAllText("Assets/Shaders/text.frag"),
            ShaderType.FragmentShader);
        
        textShader = new ShaderProgram("text");
        textShader.LinkShaders(textVert, textFrag);
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        TextDrawData textDrawData = EnsureDrawDataType<TextDrawData>(drawData);
        
        glStateManager.UseProgram(textShader.Handle);
        glStateManager.BindVertexArray(textDrawData.VertexArray.Handle);

        Matrix4 transformation = textDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
                
        for (int i = 0; i < engine.TextResources.FontTextures.Length; i++)
        {
            GL.Uniform1(i + 1, i);
            glStateManager.BindTextureUnit(i, engine.TextResources.FontTextures[i].Handle);
        }

        GL.Uniform4(17, textDrawData.Color);
                
        GL.DrawArrays(PrimitiveType.Triangles, 0, textDrawData.Count);
    }

    public override void Dispose()
    {
        textShader.Dispose();
    }
}