using System.Diagnostics;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

public class Fxaa : PostProcessor, IDisposable
{
    private ShaderProgram program;
    private Texture2D? antialiasedTexture;

    public Fxaa()
    {
        using var shader = new Shader("fxaa", File.ReadAllText("Assets/Shaders/Compute/fxaa.comp"),
            ShaderType.ComputeShader);
        program = new ShaderProgram("fxaa");
        program.LinkShaders(shader);
    }

    public override void Init(Vector2i size)
    {
        base.Init(size);

        antialiasedTexture?.Dispose();
        antialiasedTexture = new Texture2D("fxaa", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }
    
    public override void Process(GLStateManager stateManager, IRenderBuffer renderBuffer, Texture2D texture)
    {
        if (!Initialized)
            throw new InvalidOperationException($"{nameof(Fxaa)} was not initialized!");
        
        Debug.Assert(antialiasedTexture != null);
        
        stateManager.UseProgram(program);
        stateManager.BindTextureUnit(0, texture);
        GL.BindImageTexture(0, antialiasedTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(DivideIntCeil(CurrentSize.X, 8), DivideIntCeil(CurrentSize.Y, 8), 1);
        
        GL.CopyImageSubData(
            antialiasedTexture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0, 
            texture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0,
            CurrentSize.X, CurrentSize.Y, 1);
    }

    public void Dispose()
    {
        antialiasedTexture?.Dispose();
        program.Dispose();
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}