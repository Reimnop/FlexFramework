using System.Diagnostics;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

public class Exposure : PostProcessor, IDisposable
{
    public float ExposureValue { get; set; } = 1.0f;

    private ShaderProgram program;
    private Texture2D? tonemappedTexture;

    public Exposure()
    {
        using var shader = new Shader("exposure", File.ReadAllText("Assets/Shaders/Compute/exposure.comp"),
            ShaderType.ComputeShader);
        program = new ShaderProgram("exposure");
        program.LinkShaders(shader);
    }
    
    public override void Init(Vector2i size)
    {
        base.Init(size);

        tonemappedTexture?.Dispose();
        tonemappedTexture = new Texture2D("exposure", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }
    
    public override void Process(GLStateManager stateManager, IRenderBuffer renderBuffer, Texture2D texture)
    {
        if (!Initialized)
            throw new InvalidOperationException($"{nameof(Exposure)} was not initialized!");
        
        Debug.Assert(tonemappedTexture != null);
        
        stateManager.UseProgram(program);
        GL.Uniform1(program.GetUniformLocation("exposure"), ExposureValue);
        stateManager.BindTextureUnit(0, texture);
        GL.BindImageTexture(0, tonemappedTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(DivideIntCeil(CurrentSize.X, 8), DivideIntCeil(CurrentSize.Y, 8), 1);
        
        GL.CopyImageSubData(
            tonemappedTexture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0, 
            texture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0,
            CurrentSize.X, CurrentSize.Y, 1);
    }

    public void Dispose()
    {
        tonemappedTexture?.Dispose();
        program.Dispose();
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}