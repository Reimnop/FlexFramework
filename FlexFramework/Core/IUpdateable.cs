namespace FlexFramework.Core;

/// <summary>
/// Provides a method for mechanism an object
/// </summary>
public interface IUpdateable
{
    /// <summary>
    /// Update this object
    /// </summary>
    /// <param name="args">Arguments for updating the object</param>
    void Update(UpdateArgs args);
}