using FlexFramework.Core.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using Sampler = FlexFramework.Core.Rendering.Data.Sampler;
using Timer = FlexFramework.Util.Timer;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class SamplerHandler : IDisposable
{
    private readonly GarbageCollector<ISamplerView, Sampler> gc;
    private readonly Timer timer;

    public SamplerHandler()
    {
        gc = new GarbageCollector<ISamplerView, Sampler>(GetHash, CreateSampler);
        timer = new Timer(1.0f, () => gc.Sweep());
    }

    public void Update(float deltaTime)
    {
        timer.Update(deltaTime);
    }
    
    public Sampler GetSampler(ISamplerView sampler)
    {
        return gc.GetOrAllocate(sampler);
    }
    
    private static Hash128 GetHash(ISamplerView sampler)
    {
        return sampler.Hash;
    }
    
    private Sampler CreateSampler(ISamplerView sampler)
    {
        var filterMode = ConvertToTextureFilter(sampler.FilterMode);
        var wrapMode = ConvertToTextureWrap(sampler.WrapMode);
        var samp = new Sampler(sampler.Name);
        samp.Parameter(SamplerParameterName.TextureMinFilter, filterMode);
        samp.Parameter(SamplerParameterName.TextureMagFilter, filterMode);
        samp.Parameter(SamplerParameterName.TextureWrapS, wrapMode);
        samp.Parameter(SamplerParameterName.TextureWrapT, wrapMode);
        samp.Parameter(SamplerParameterName.TextureWrapR, wrapMode);
        
        Span<float> borderColor = stackalloc float[4];
        borderColor[0] = sampler.BorderColor.R;
        borderColor[1] = sampler.BorderColor.G;
        borderColor[2] = sampler.BorderColor.B;
        borderColor[3] = sampler.BorderColor.A;
        samp.Parameter(SamplerParameterName.TextureBorderColor, borderColor);
        
        return samp;
    }

    private static int ConvertToTextureFilter(FilterMode filterMode)
    {
        return filterMode switch
        {
            FilterMode.Linear => (int) All.Linear,
            FilterMode.Nearest => (int) All.Nearest,
            _ => throw new ArgumentException(null, nameof(filterMode))
        };
    }

    private static int ConvertToTextureWrap(WrapMode wrapMode)
    {
        return wrapMode switch
        {
            WrapMode.Repeat => (int) TextureWrapMode.Repeat,
            WrapMode.MirroredRepeat => (int) TextureWrapMode.MirroredRepeat,
            WrapMode.ClampToEdge => (int) TextureWrapMode.ClampToEdge,
            WrapMode.ClampToBorder => (int) TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentException(null, nameof(wrapMode))
        };
    }

    public void Dispose()
    {
        gc.Dispose();
    }
}