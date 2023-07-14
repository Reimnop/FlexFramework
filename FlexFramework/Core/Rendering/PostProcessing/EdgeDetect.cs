using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

public class EdgeDetect : PostProcessor
{
    private ShaderProgram program;
    private Texture2D outputTexture;

    public EdgeDetect()
    {
        using var shader = new Shader("sobel", File.ReadAllText("Assets/Shaders/Compute/sobel.comp"), ShaderType.ComputeShader);
        program = new ShaderProgram("sobel");
        program.LinkShaders(shader);
    }
    
    public override void Resize(Vector2i size)
    {
        base.Resize(size);
        
        outputTexture.Dispose();
        outputTexture = new Texture2D("sobel", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }

    public override void Init(Vector2i size)
    {
        base.Init(size);

        outputTexture = new Texture2D("sobel", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }
    
    // We don't need the render buffer here
    public override void Process(GLStateManager stateManager, IRenderBuffer renderBuffer, Texture2D texture)
    {
        if (renderBuffer is not IGBuffer gBuffer)
        {
            return;
        }
        
        stateManager.UseProgram(program);
        GL.Uniform1(program.GetUniformLocation("positionTexture"), 0);
        GL.Uniform1(program.GetUniformLocation("normalTexture"), 1);
        GL.Uniform1(program.GetUniformLocation("colorTexture"), 2);

        stateManager.BindTextureUnit(0, gBuffer.WorldPosition);
        stateManager.BindTextureUnit(1, gBuffer.WorldNormal);
        stateManager.BindTextureUnit(2, texture);
        GL.BindImageTexture(0, outputTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(DivideIntCeil(CurrentSize.X, 8), DivideIntCeil(CurrentSize.Y, 8), 1);
        
        GL.CopyImageSubData(
            outputTexture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0, 
            texture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0,
            CurrentSize.X, CurrentSize.Y, 1);
    }

    public void Dispose()
    {
        outputTexture.Dispose();
        program.Dispose();
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}