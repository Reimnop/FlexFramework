namespace FlexFramework.Core.Rendering.Data;

public class ProgramUniformNotFoundException : Exception
{
    public ProgramUniformNotFoundException(string programName, string attributeName) : base($"Uniform '{attributeName}' not found in program '{programName}'!")
    {
    }
}