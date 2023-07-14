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
    
    public override void Render(CommandList commandList, GLStateManager stateManager, IRenderBuffer renderBuffer, CameraData cameraData)
    {
        if (renderBuffer is not IGBuffer gBuffer)
            return;
        
        if (!commandList.TryGetLighting(out var lighting))
            return;
        
        var directionalLight = lighting.GetDirectionalLight();
        if (directionalLight.Direction == Vector3.Zero)
            return;

        stateManager.UseProgram(program);

        Matrix4 inverseView = Matrix4.Invert(cameraData.View);
        Matrix4 inverseProjection = Matrix4.Invert(cameraData.Projection);
        
        GL.UniformMatrix4(program.GetUniformLocation("inverseProjection"), true, ref inverseProjection);
        GL.UniformMatrix4(program.GetUniformLocation("inverseView"), true, ref inverseView);
        GL.Uniform3(program.GetUniformLocation("lightDir"), directionalLight.Direction);
        GL.Uniform3(program.GetUniformLocation("lightColor"), (directionalLight.Color * directionalLight.Intensity + lighting.GetAmbientLight()) * 2.0f);

        GL.BindImageTexture(0, gBuffer.WorldColor.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.BindImageTexture(1, gBuffer.WorldNormal.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.BindImageTexture(2, gBuffer.WorldPosition.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(MathUtil.DivideIntCeil(renderBuffer.Size.X, 8), MathUtil.DivideIntCeil(renderBuffer.Size.Y, 8), 1);
    }

    public void Dispose()
    {
        program.Dispose();
    }
}