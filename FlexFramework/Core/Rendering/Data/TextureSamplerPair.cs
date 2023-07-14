using FlexFramework.Core.Data;

namespace FlexFramework.Core.Rendering.Data;

public struct TextureSamplerPair
{
    public ITextureView Texture { get; }
    public ISamplerView Sampler { get; }
    
    public TextureSamplerPair(ITextureView texture, ISamplerView sampler)
    {
        Texture = texture;
        Sampler = sampler;
    }
}