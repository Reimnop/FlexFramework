using FlexFramework.Util.Exceptions;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class ShaderProgram : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public ShaderProgram(string name)
    {
        Name = name;

        Handle = GL.CreateProgram();
        GL.ObjectLabel(ObjectLabelIdentifier.Program, Handle, name.Length, name);
    }

    public void AttachShader(Shader shader)
    {
        GL.AttachShader(Handle, shader.Handle);
    }
    
    public void DetachShader(Shader shader)
    {
        GL.DetachShader(Handle, shader.Handle);
    }

    public void Link()
    {
        GL.LinkProgram(Handle);
    }

    public void LinkShaders(params Shader[] shaders)
    {
        foreach (Shader shader in shaders)
        {
            AttachShader(shader);
        }
        Link();
        foreach (Shader shader in shaders)
        {
            DetachShader(shader);
        }
        
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
        if (status != (int) All.True)
        {
            string message = GL.GetProgramInfoLog(Handle);
            throw new ProgramLinkingException(Name, message);
        }
    }
    
    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}