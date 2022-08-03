using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Util.Exceptions;

public class ShaderCompilationException : Exception
{
    public ShaderCompilationException(ShaderType shaderType, string name, string message) : base($"Failed to compile shader '{name}' of type '{shaderType}': {message}")
    {
    }
}