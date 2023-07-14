using FlexFramework.Core.Rendering;

namespace FlexFramework.Core;

/// <summary>
/// Container for arguments for rendering an object
/// </summary>
public struct RenderArgs
{
    /// <summary>
    /// Command list to queue rendering commands
    /// </summary>
    public CommandList CommandList { get; }
    
    /// <summary>
    /// What layer this object is being rendered on
    /// </summary>
    public LayerType LayerType { get; }
    
    /// <summary>
    /// Matrix stack for managing transformations
    /// </summary>
    public MatrixStack MatrixStack { get; }
    
    /// <summary>
    /// Information about the camera
    /// </summary>
    public CameraData CameraData { get; }
    
    public RenderArgs(CommandList commandList, LayerType layerType, MatrixStack matrixStack, CameraData cameraData)
    {
        CommandList = commandList;
        LayerType = layerType;
        MatrixStack = matrixStack;
        CameraData = cameraData;
    }
}