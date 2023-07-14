using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering;

public class GLStateManager
{
    private const int MaxTextureUnits = 16;
    
    private FrameBuffer? currentFramebuffer = null;
    private ShaderProgram? currentProgram = null;
    private VertexArray? currentVertexArray = null;
    private Texture2D?[] currentTextureUnits = new Texture2D?[MaxTextureUnits];
    private Sampler?[] currentSamplers = new Sampler?[MaxTextureUnits];
    
    private bool depthMask = true;

    private readonly Dictionary<EnableCap, bool> glCapabilities = new Dictionary<EnableCap, bool>();
    
    public void SetDepthMask(bool enabled)
    {
        if (depthMask != enabled)
        {
            GL.DepthMask(enabled);
            depthMask = enabled;
        }
    }

    public void SetCapability(EnableCap cap, bool enabled)
    {
        if (!glCapabilities.TryGetValue(cap, out bool currentlyEnabled))
        {
            SetCapabilityInternal(cap, enabled);
            glCapabilities.Add(cap, enabled);
            return;
        }
        
        if (currentlyEnabled == enabled)
        {
            return;
        }

        glCapabilities[cap] = enabled;
        SetCapabilityInternal(cap, enabled);
    }

    private void SetCapabilityInternal(EnableCap cap, bool enabled)
    {
        if (enabled)
        {
            GL.Enable(cap);
        }
        else
        {
            GL.Disable(cap);
        }
    }

    public void BindFramebuffer(FrameBuffer? framebuffer)
    {
        if (currentFramebuffer == framebuffer)
        {
            return;
        }

        currentFramebuffer = framebuffer;
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer?.Handle ?? 0);
    }

    public void UseProgram(ShaderProgram? program)
    {
        if (currentProgram == program)
        {
            return;
        }
        
        currentProgram = program;
        GL.UseProgram(program?.Handle ?? 0);
    }
    
    public void BindVertexArray(VertexArray vertexArray)
    {
        if (currentVertexArray == vertexArray)
        {
            return;
        }
        
        currentVertexArray = vertexArray;
        GL.BindVertexArray(vertexArray.Handle);
    }
    
    public void BindTextureUnit(int unit, Texture2D? texture)
    {
        if (unit < 0 || unit >= MaxTextureUnits)
        {
            throw new ArgumentOutOfRangeException(nameof(unit));
        }
        
        if (currentTextureUnits[unit] == texture)
        {
            return;
        }
        
        currentTextureUnits[unit] = texture;
        GL.BindTextureUnit(unit, texture?.Handle ?? 0);
    }
    
    public void BindSampler(int unit, Sampler? sampler)
    {
        if (unit < 0 || unit >= MaxTextureUnits)
        {
            throw new ArgumentOutOfRangeException(nameof(unit));
        }
        
        if (currentSamplers[unit] == sampler)
        {
            return;
        }
        
        currentSamplers[unit] = sampler;
        GL.BindSampler(unit, sampler?.Handle ?? 0);
    }
}