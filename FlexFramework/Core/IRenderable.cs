using FlexFramework.Core;
using FlexFramework.Core.Rendering;

namespace FlexFramework.Core;

/// <summary>
/// Provides a method for rendering an object
/// </summary>
public interface IRenderable
{
    /// <summary>
    /// Render this object
    /// </summary>
    /// <param name="args">Arguments for rendering the object</param>
    void Render(RenderArgs args);
}