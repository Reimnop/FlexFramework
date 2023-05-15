using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

// TODO: Make mip count a const
public class Bloom : PostProcessor, IDisposable
{
    public float HardThreshold { get; set; } = 0.85f;
    public float SoftThreshold { get; set; } = 0.75f;
    public float Strength { get; set; } = 0.8f;

    private readonly ShaderProgram prefilterShader;
    private readonly ShaderProgram downsampleShader;
    private readonly ShaderProgram upsampleShader;
    private readonly ShaderProgram combineShader;

    private Texture2D[] downsampleMipChain;
    private Texture2D[] upsampleMipChain;
    private Texture2D prefilteredTexture;
    private Texture2D smallestTexture;
    private Texture2D finalTexture;
    
    public Bloom()
    {
        prefilterShader = LoadComputeShader("bloom-prefilter", "Assets/Shaders/Compute/bloom_prefilter.comp");
        downsampleShader = LoadComputeShader("bloom-downsample", "Assets/Shaders/Compute/bloom_downsample.comp");
        upsampleShader = LoadComputeShader("bloom-upsample", "Assets/Shaders/Compute/bloom_upsample.comp");
        combineShader = LoadComputeShader("bloom-combine", "Assets/Shaders/Compute/bloom_combine.comp");
    }
    
    public override void Resize(Vector2i size)
    {
        base.Resize(size);
        
        DeleteTextures();
        InitSize(size);
    }

    public override void Init(Vector2i size)
    {
        base.Init(size);
        InitSize(size);
    }

    private void InitSize(Vector2i size)
    {
        prefilteredTexture = InitNewTexture("bloom-prefiltered", size.X, size.Y);
        downsampleMipChain = InitMipChain(size / 2, 0.5f, 5);
        smallestTexture = InitNewTexture("bloom-smallest", downsampleMipChain[^1].Width / 2, downsampleMipChain[^1].Height / 2);
        upsampleMipChain = InitMipChain(new Vector2i(smallestTexture.Width, smallestTexture.Height) * 2, 2.0f, 5);
        finalTexture = InitNewTexture("bloom-final", size.X, size.Y);
    }

    private Texture2D[] InitMipChain(Vector2i initialSize, float factor, int mipCount)
    {
        Texture2D[] mipChain = new Texture2D[mipCount];
        for (int i = 0; i < mipCount; i++)
        {
            float multiplier = MathF.Pow(factor, i);
            Vector2i size = new Vector2i(
                (int) MathF.Ceiling(initialSize.X * multiplier), 
                (int) MathF.Ceiling(initialSize.Y * multiplier));
            mipChain[i] = InitNewTexture($"bloom-mipchain-{i}", size.X, size.Y);
        }

        return mipChain;
    }

    private Texture2D InitNewTexture(string name, int width, int height)
    {
        Texture2D texture = new Texture2D(name, width, height, SizedInternalFormat.Rgba16f);
        texture.SetMinFilter(TextureMinFilter.Linear);
        texture.SetMagFilter(TextureMagFilter.Linear);
        texture.SetWrapS(TextureWrapMode.ClampToEdge);
        texture.SetWrapT(TextureWrapMode.ClampToEdge);

        return texture;
    }

    private ShaderProgram LoadComputeShader(string name, string path)
    {
        using Shader shader = new Shader(name, File.ReadAllText(path), ShaderType.ComputeShader);
        ShaderProgram program = new ShaderProgram(name);
        program.LinkShaders(shader);
        return program;
    }

    public override void Process(GLStateManager stateManager, Texture2D texture)
    {
        stateManager.UseProgram(prefilterShader);
        GL.Uniform1(1, HardThreshold);
        GL.Uniform1(2, SoftThreshold);
        stateManager.BindTextureUnit(0, texture);
        GL.BindImageTexture(0, prefilteredTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.DispatchCompute(DivideIntCeil(CurrentSize.X, 8), DivideIntCeil(CurrentSize.Y, 8), 1);

        stateManager.UseProgram(downsampleShader);
        GL.Uniform1(2, 0);
        
        for (int i = 0; i < 6; i++)
        {
            Texture2D inputTexture = i == 0 ? prefilteredTexture : downsampleMipChain[i - 1];
            Texture2D outputTexture = i == 5 ? smallestTexture : downsampleMipChain[i];
            
            stateManager.BindTextureUnit(0, inputTexture);
            GL.Uniform2(1, 1.0f / inputTexture.Width, 1.0f / inputTexture.Height); // input texture pixel size
            GL.BindImageTexture(0, outputTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DispatchCompute(DivideIntCeil(outputTexture.Width, 8), DivideIntCeil(outputTexture.Height, 8), 1);
        }
        
        stateManager.UseProgram(upsampleShader);
        GL.Uniform1(0, 0);
        GL.Uniform1(1, 1);
        GL.Uniform1(2, Strength);
        
        for (int i = 0; i < 5; i++)
        {
            Texture2D inputTexture1 = i == 0 ? smallestTexture : upsampleMipChain[i - 1];
            Texture2D inputTexture2 = downsampleMipChain[^(i + 1)];
            Texture2D outputTexture = upsampleMipChain[i];

            stateManager.BindTextureUnit(0, inputTexture1);
            stateManager.BindTextureUnit(1, inputTexture2);
            GL.BindImageTexture(0, outputTexture.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DispatchCompute(DivideIntCeil(outputTexture.Width, 8), DivideIntCeil(outputTexture.Height, 8), 1);
        }
        
        stateManager.UseProgram(combineShader);
        GL.Uniform1(0, 0);
        GL.Uniform1(1, 1);
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
        prefilterShader.Dispose();
        downsampleShader.Dispose();
        upsampleShader.Dispose();
        combineShader.Dispose();
        DeleteTextures();
    }

    private void DeleteTextures()
    {
        List<Texture2D> textures = new List<Texture2D>();
        textures.AddRange(downsampleMipChain);
        textures.AddRange(upsampleMipChain);
        textures.Add(prefilteredTexture);
        textures.Add(smallestTexture);
        textures.Add(finalTexture);
        textures.ForEach(x => x.Dispose());
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}