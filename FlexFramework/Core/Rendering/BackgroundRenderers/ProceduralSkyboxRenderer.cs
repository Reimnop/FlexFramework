using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.BackgroundRenderers;

public class ProceduralSkyboxRenderer : BackgroundRenderer, IDisposable
{
    private readonly ShaderProgram program;
    
    public ProceduralSkyboxRenderer()
    {
        using Shader shader = new Shader("skybox", File.ReadAllText("Assets/Shaders/Compute/procedural_skybox.comp"), ShaderType.ComputeShader);
        
        program = new ShaderProgram("skybox");
        program.LinkShaders(shader);
    }
    
    public override void Render(Renderer renderer, GLStateManager stateManager, Texture2D renderTarget, CameraData cameraData)
    {
        if (renderer is not ILighting lighting)
        {
            return;
        }

        if (!lighting.DirectionalLight.HasValue)
        {
            return;
        }
        
        stateManager.UseProgram(program);

        Matrix4 inverseView = Matrix4.Invert(cameraData.View);
        Matrix4 inverseProjection = Matrix4.Invert(cameraData.Projection);
        
        GL.UniformMatrix4(0, true, ref inverseProjection);
        GL.UniformMatrix4(1, true, ref inverseView);
        GL.Uniform3(2, lighting.DirectionalLight.Value.Direction);
        GL.Uniform3(3, (lighting.DirectionalLight.Value.Color * lighting.DirectionalLight.Value.Intensity + lighting.AmbientLight) * 2.0f);

        GL.BindImageTexture(0, renderTarget.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(MathUtil.DivideIntCeil(renderTarget.Width, 8), MathUtil.DivideIntCeil(renderTarget.Height, 8), 1);
    }

    public void Dispose()
    {
        program.Dispose();
    }
}