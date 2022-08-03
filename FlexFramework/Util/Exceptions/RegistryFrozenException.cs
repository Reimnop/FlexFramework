namespace FlexFramework.Util.Exceptions;

public class RegistryFrozenException : Exception
{
    public RegistryFrozenException() : base("Could not register to a frozen registry")
    {
    }
}