using FlexFramework.Util.Exceptions;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class ShaderProgram : IGpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }
    
    private readonly Dictionary<string, int> attributeLocations = new();
    private readonly Dictionary<string, int> uniformLocations = new();

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
    
    public int GetAttributeLocation(string name)
    {
        if (attributeLocations.TryGetValue(name, out var location))
            return location;
        
        location = GL.GetAttribLocation(Handle, name);
        if (location == -1)
            throw new ProgramAttributeNotFoundException(Name, name);
        
        attributeLocations.Add(name, location);
        return location;
    }
    
    public int GetUniformLocation(string name)
    {
        if (uniformLocations.TryGetValue(name, out var location))
            return location;
        
        location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
            throw new ProgramUniformNotFoundException(Name, name);
        
        uniformLocations.Add(name, location);
        return location;
    }

    public void LinkShaders(params Shader[] shaders)
    {
        foreach (var shader in shaders)
        {
            AttachShader(shader);
        }
        Link();
        foreach (var shader in shaders)
        {
            DetachShader(shader);
        }
        
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
        if (status != (int) All.True)
        {
            var message = GL.GetProgramInfoLog(Handle);
            throw new ProgramLinkingException(Name, message);
        }
    }
    
    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}