using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public class Sampler : DataObject
{
    private struct ReadOnlySampler : ISamplerView
    {
        public string Name => sampler.Name;
        public FilterMode FilterMode => sampler.FilterMode;
        public WrapMode WrapMode => sampler.WrapMode;
        public Color4 BorderColor => sampler.BorderColor;
        public Hash128 Hash => sampler.Hash;

        private readonly Sampler sampler;
        
        public ReadOnlySampler(Sampler sampler)
        {
            this.sampler = sampler;
        }
    }

    public FilterMode FilterMode
    {
        get => filterMode;
        set
        {
            if (readOnly)
                throw new InvalidOperationException("Cannot modify read-only sampler!");
            
            filterMode = value;
        }
    }

    public WrapMode WrapMode
    {
        get => wrapMode;
        set
        {
            if (readOnly)
                throw new InvalidOperationException("Cannot modify read-only sampler!");
            
            wrapMode = value;
        }
    }

    public Color4 BorderColor
    {
        get => borderColor;
        set
        {
            if (readOnly)
                throw new InvalidOperationException("Cannot modify read-only sampler!");
            
            borderColor = value;
        }
    }
    
    private FilterMode filterMode = FilterMode.Linear;
    private WrapMode wrapMode = WrapMode.Repeat;
    private Color4 borderColor = Color4.Black;
    
    public Hash128 Hash => HashUtil.Hash((int) filterMode) ^ HashUtil.Hash((int) wrapMode) ^ HashUtil.Hash(borderColor);

    public ISamplerView ReadOnly => new ReadOnlySampler(this);
    
    private bool readOnly;
    
    public Sampler(string name) : base(name)
    {
    }
    
    public Sampler SetReadOnly()
    {
        readOnly = true;
        return this;
    }
}