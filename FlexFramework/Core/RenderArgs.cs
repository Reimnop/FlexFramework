using FlexFramework.Core.Rendering;

namespace FlexFramework.Core;

public struct RenderArgs
{
    public CommandList CommandList { get; }
    public LayerType LayerType { get; }
    public MatrixStack MatrixStack { get; }
    public CameraData CameraData { get; }
    
    public RenderArgs(CommandList commandList, LayerType layerType, MatrixStack matrixStack, CameraData cameraData)
    {
        CommandList = commandList;
        LayerType = layerType;
        MatrixStack = matrixStack;
        CameraData = cameraData;
    }
}