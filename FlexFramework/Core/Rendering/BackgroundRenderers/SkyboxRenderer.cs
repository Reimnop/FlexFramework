using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.BackgroundRenderers;

public class SkyboxRenderer : BackgroundRenderer, IDisposable
{
    public Texture2D? Texture { get; set; }
    
    private readonly ShaderProgram program;
    
    public SkyboxRenderer()
    {
        using Shader shader = new Shader("skybox", File.ReadAllText("Assets/Shaders/Compute/skybox.comp"), ShaderType.ComputeShader);
        
        program = new ShaderProgram("skybox");
        program.LinkShaders(shader);
    }
    
    public override void Render(Renderer renderer, GLStateManager stateManager, Texture2D renderTarget, CameraData cameraData)
    {
        if (Texture == null)
        {
            return;
        }
        
        stateManager.UseProgram(program);
        stateManager.BindTextureUnit(0, Texture);
            
        Matrix4 inverseView = Matrix4.Invert(cameraData.View);
        Matrix4 inverseProjection = Matrix4.Invert(cameraData.Projection);
            
        GL.UniformMatrix4(1, true, ref inverseProjection);
        GL.UniformMatrix4(2, true, ref inverseView);
            
        GL.BindImageTexture(0, renderTarget.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(MathUtil.DivideIntCeil(renderTarget.Width, 8), MathUtil.DivideIntCeil(renderTarget.Height, 8), 1);
    }

    public void Dispose()
    {
        program.Dispose();
    }
}