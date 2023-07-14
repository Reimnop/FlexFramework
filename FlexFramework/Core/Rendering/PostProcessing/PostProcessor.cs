using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.PostProcessing;

/// <summary>
/// Base class for post processing effects
/// </summary>
public abstract class PostProcessor
{
    /// <summary>
    /// Size of the current internal render buffers
    /// This must be the same size as the render target
    /// </summary>
    public Vector2i CurrentSize { get; private set; }

    /// <summary>
    /// Resize the internal render buffers
    /// </summary>
    /// <param name="size">New size of the internal buffers</param>
    public virtual void Resize(Vector2i size)
    {
        CurrentSize = size;
    }
    
    /// <summary>
    /// Initialize the internal render buffers
    /// This must be run before using the post processor
    /// </summary>
    /// <param name="size">Initial size of the internal buffers</param>
    public virtual void Init(Vector2i size)
    {
        CurrentSize = size;
    }
    
    
    /// <summary>
    /// Run the post processor
    /// </summary>
    /// <param name="stateManager">State manager for the current GL context</param>
    /// <param name="renderBuffer">Information about the rendered scene</param>
    /// <param name="texture">The texture to write the processed image into</param>
    public abstract void Process(GLStateManager stateManager, IRenderBuffer renderBuffer, Texture2D texture);
}