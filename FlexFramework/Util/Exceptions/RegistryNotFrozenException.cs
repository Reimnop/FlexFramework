namespace FlexFramework.Util.Exceptions;

public class RegistryNotFrozenException : Exception
{
    public RegistryNotFrozenException() : base("Could not access a non frozen registry")
    {
    }
}