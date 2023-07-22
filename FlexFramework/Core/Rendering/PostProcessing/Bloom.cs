using System.Diagnostics;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

public class Bloom : PostProcessor, IDisposable
{
    private const int MipCount = 6;
    
    public float HardThreshold { get; set; } = 0.85f;
    public float SoftThreshold { get; set; } = 0.75f;
    public float Strength { get; set; } = 0.8f;

    private readonly ShaderProgram prefilterShader;
    private readonly ShaderProgram downsampleShader;
    private readonly ShaderProgram upsampleShader;
    private readonly ShaderProgram combineShader;
    private readonly Sampler sampler;

    private Texture2D[]? downsampleMipChain;
    private Texture2D[]? upsampleMipChain;
    private Texture2D? prefilteredTexture;
    private Texture2D? smallestTexture;
    private Texture2D? finalTexture;

    public Bloom()
    {
        prefilterShader = LoadComputeShader("bloom_prefilter", "Assets/Shaders/Compute/bloom_prefilter.comp");
        downsampleShader = LoadComputeShader("bloom_downsample", "Assets/Shaders/Compute/bloom_downsample.comp");
        upsampleShader = LoadComputeShader("bloom_upsample", "Assets/Shaders/Compute/bloom_upsample.comp");
        combineShader = LoadComputeShader("bloom_combine", "Assets/Shaders/Compute/bloom_combine.comp");
        
        sampler = new Sampler("bloom_sampler");
        sampler.Parameter(SamplerParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
        sampler.Parameter(SamplerParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        sampler.Parameter(SamplerParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
        sampler.Parameter(SamplerParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
    }
    
    public override void Init(Vector2i size)
    {
        base.Init(size);
        
        DeleteTextures();
        prefilteredTexture = new Texture2D("bloom_prefiltered", size.X, size.Y, SizedInternalFormat.Rgba16f);
        downsampleMipChain = InitMipChain(size / 2, 0.5f, 5);
        smallestTexture = new Texture2D("bloom_smallest", downsampleMipChain[^1].Width / 2, downsampleMipChain[^1].Height / 2, SizedInternalFormat.Rgba16f);
        upsampleMipChain = InitMipChain(new Vector2i(smallestTexture.Width, smallestTexture.Height) * 2, 2.0f, 5);
        finalTexture = new Texture2D("bloom_final", size.X, size.Y, SizedInternalFormat.Rgba16f);
    }

    private static Texture2D[] InitMipChain(Vector2i initialSize, float factor, int mipCount)
    {
        var mipChain = new Texture2D[mipCount];
        for (int i = 0; i < mipCount; i++)
        {
            float multiplier = MathF.Pow(factor, i);
            Vector2i size = new Vector2i(
                (int) MathF.Ceiling(initialSize.X * multiplier), 
                (int) MathF.Ceiling(initialSize.Y * multiplier));
            mipChain[i] = new Texture2D($"bloom_mipchain[{i}]", size.X, size.Y, SizedInternalFormat.Rgba16f);
        }

        return mipChain;
    }

    private ShaderProgram LoadComputeShader(string name, string path)
    {
        using var shader = new Shader(name, File.ReadAllText(path), ShaderType.ComputeShader);
        var program = new ShaderProgram(name);
        program.LinkShaders(shader);
        return program;
    }

    public override void Process(GLStateManager stateManager, IRenderBuffer renderBuffer, Texture2D texture)
    {
        if (!Initialized)
            throw new InvalidOperationException($"{nameof(Bloom)} was not initialized!");
        
        Debug.Assert(downsampleMipChain != null);
        Debug.Assert(upsampleMipChain != null);
        Debug.Assert(prefilteredTexture != null);
        Debug.Assert(smallestTexture != null);
        Debug.Assert(finalTexture != null);
        
        stateManager.BindSampler(0, sampler);
        
        // Prefiltering
        stateManager.UseProgram(prefilterShader);
        GL.Uniform1(prefilterShader.GetUniformLocation("hardThreshold"), HardThreshold);
        GL.Uniform1(prefilterShader.GetUniformLocation("softThreshold"), SoftThreshold);
        stateManager.BindTextureUnit(0, texture);
        GL.BindImageTexture(0, prefilteredTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(DivideIntCeil(CurrentSize.X, 8), DivideIntCeil(CurrentSize.Y, 8), 1);

        // Downsampling
        stateManager.UseProgram(downsampleShader);
        GL.Uniform1(downsampleShader.GetUniformLocation("inputTexture"), 0);
        
        for (int i = 0; i < MipCount; i++)
        {
            var inputTexture = i == 0 ? prefilteredTexture : downsampleMipChain[i - 1];
            var outputTexture = i == MipCount - 1 ? smallestTexture : downsampleMipChain[i];
            
            stateManager.BindTextureUnit(0, inputTexture); 
            GL.BindImageTexture(0, outputTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DispatchCompute(DivideIntCeil(outputTexture.Width, 8), DivideIntCeil(outputTexture.Height, 8), 1);
        }
        
        // Upsampling
        stateManager.UseProgram(upsampleShader);
        GL.Uniform1(upsampleShader.GetUniformLocation("inputTexture1"), 0);
        GL.Uniform1(upsampleShader.GetUniformLocation("inputTexture2"), 1);
        GL.Uniform1(upsampleShader.GetUniformLocation("strength"), Strength);
        
        for (int i = 0; i < MipCount - 1; i++)
        {
            var inputTexture1 = i == 0 ? smallestTexture : upsampleMipChain[i - 1];
            var inputTexture2 = downsampleMipChain[^(i + 1)];
            var outputTexture = upsampleMipChain[i];

            stateManager.BindTextureUnit(0, inputTexture1);
            stateManager.BindTextureUnit(1, inputTexture2);
            GL.BindImageTexture(0, outputTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DispatchCompute(DivideIntCeil(outputTexture.Width, 8), DivideIntCeil(outputTexture.Height, 8), 1);
        }
        
        // Combining
        stateManager.UseProgram(combineShader);
        GL.Uniform1(combineShader.GetUniformLocation("inputTexture1"), 0);
        GL.Uniform1(combineShader.GetUniformLocation("inputTexture2"), 1);
        stateManager.BindTextureUnit(0, upsampleMipChain[^1]);
        stateManager.BindTextureUnit(1, texture);
        GL.BindImageTexture(0, finalTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(DivideIntCeil(CurrentSize.X, 8), DivideIntCeil(CurrentSize.Y, 8), 1);

        // Copy final to texture
        GL.CopyImageSubData(
            finalTexture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0, 
            texture.Handle, ImageTarget.Texture2D, 0, 0, 0, 0,
            CurrentSize.X, CurrentSize.Y, 1);
    }

    public void Dispose()
    {
        DeleteShaders();
        DeleteTextures();
    }

    private void DeleteShaders()
    {
        prefilterShader.Dispose();
        downsampleShader.Dispose();
        upsampleShader.Dispose();
        combineShader.Dispose();
        sampler.Dispose();
    }

    private void DeleteTextures()
    {
        if (downsampleMipChain != null)
            foreach (var texture in downsampleMipChain)
                texture.Dispose();
        if (upsampleMipChain != null)
            foreach (var texture in upsampleMipChain)
                texture.Dispose();
        
        prefilteredTexture?.Dispose();
        smallestTexture?.Dispose();
        finalTexture?.Dispose();
    }

    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}