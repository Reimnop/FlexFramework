using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class Sampler : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }
    
    public Sampler(string name)
    {
        Name = name;

        GL.CreateSamplers(1, out int handle);
        GL.ObjectLabel(ObjectLabelIdentifier.Sampler, handle, name.Length, name);
        Handle = handle;
    }
    
    public void Parameter(SamplerParameterName samplerParameterName, int value)
    {
        GL.SamplerParameter(Handle, samplerParameterName, value);
    }
    
    public void Parameter(SamplerParameterName samplerParameterName, float value)
    {
        GL.SamplerParameter(Handle, samplerParameterName, value);
    }
    
    public unsafe void Parameter(SamplerParameterName samplerParameterName, ReadOnlySpan<int> value)
    {
        fixed (int* ptr = value)
        {
            GL.SamplerParameter(Handle, samplerParameterName, ptr);
        }
    }
    
    public unsafe void Parameter(SamplerParameterName samplerParameterName, ReadOnlySpan<float> value)
    {
        fixed (float* ptr = value)
        {
            GL.SamplerParameter(Handle, samplerParameterName, ptr);
        }
    }

    public void Dispose()
    {
        GL.DeleteSampler(Handle);
    }
}