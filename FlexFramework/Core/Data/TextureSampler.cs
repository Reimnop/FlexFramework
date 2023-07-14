using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public class TextureSampler
{
    public Texture Texture { get; }
    public Sampler Sampler { get; }
    
    public int Width => Texture.Width;
    public int Height => Texture.Height;
    
    public WrapMode WrapMode
    {
        get => Sampler.WrapMode;
        set => Sampler.WrapMode = value;
    }
    
    public FilterMode FilterMode
    {
        get => Sampler.FilterMode;
        set => Sampler.FilterMode = value;
    }
    
    public Color4 BorderColor
    {
        get => Sampler.BorderColor;
        set => Sampler.BorderColor = value;
    }

    public TextureSampler(Texture texture, Sampler sampler)
    {
        Texture = texture;
        Sampler = sampler;
    }

    public TextureSampler SetData<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        Texture.SetData(data);
        return this;
    }

    public TextureSampler SetWrapMode(WrapMode wrapMode)
    {
        Sampler.WrapMode = wrapMode;
        return this;
    }
    
    public TextureSampler SetFilterMode(FilterMode filterMode)
    {
        Sampler.FilterMode = filterMode;
        return this;
    }
    
    public TextureSampler SetBorderColor(Color4 borderColor)
    {
        Sampler.BorderColor = borderColor;
        return this;
    }

    public static TextureSampler FromFile(string name, string path)
    {
        var sampler = new Sampler(name);
        var texture = Texture.FromFile(name, path);
        return new TextureSampler(texture, sampler);
    }

    public static explicit operator TextureSamplerPair?(TextureSampler? sampler)
    {
        return sampler != null ? new TextureSamplerPair(sampler.Texture.ReadOnly, sampler.Sampler.ReadOnly) : null;
    }
    
    public static explicit operator TextureSamplerPair(TextureSampler sampler)
    {
        return new TextureSamplerPair(sampler.Texture.ReadOnly, sampler.Sampler.ReadOnly);
    }
}