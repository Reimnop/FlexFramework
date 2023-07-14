using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

public class Reinhard : PostProcessor, IDisposable
{
    private ShaderProgram program;
    private Texture2D tonemappedTexture;

    public Reinhard()
    {
        using var shader = new Shader("reinhard", File.ReadAllText("Assets/Shaders/Compute/reinhard.comp"),
            ShaderType.ComputeShader);
        program = new ShaderProgram("reinhard");
        program.LinkShaders(shader);
    }
    
    public override void Resize(Vector2i size)
    {
        base.Resize(size);
        
        tonemappedTexture.Dispose();
        tonemappedTexture = new Texture2D("reinhard", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }

    public override void Init(Vector2i size)
    {
        base.Init(size);

        tonemappedTexture = new Texture2D("reinhard", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }
    
    // We don't need the render buffer here
    public override void Process(GLStateManager stateManager, IRenderBuffer renderBuffer, Texture2D texture)
    {
        stateManager.UseProgram(program);
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
        tonemappedTexture.Dispose();
        program.Dispose();
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}