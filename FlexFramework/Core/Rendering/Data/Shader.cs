using FlexFramework.Util.Exceptions;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class Shader : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public ShaderType ShaderType { get; }

    public Shader(string name, string code, ShaderType shaderType)
    {
        Name = name;
        ShaderType = shaderType;

        Handle = GL.CreateShader(shaderType);
        GL.ShaderSource(Handle, code);
        GL.CompileShader(Handle);
        
        GL.ObjectLabel(ObjectLabelIdentifier.Shader, Handle, name.Length, name);
        
        GL.GetShader(Handle, ShaderParameter.CompileStatus, out int status);
        if (status != (int) All.True)
        {
            GL.GetShaderInfoLog(Handle, out string message);
            throw new ShaderCompilationException(shaderType, name, message);
        }
    }

    public void Dispose()
    {
        GL.DeleteShader(Handle);
    }
}