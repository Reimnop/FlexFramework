namespace FlexFramework.Core.Rendering.Data;

public class ProgramAttributeNotFoundException : Exception
{
    public ProgramAttributeNotFoundException(string programName, string attributeName) : base($"Attribute '{attributeName}' not found in program '{programName}'!")
    {
    }
}