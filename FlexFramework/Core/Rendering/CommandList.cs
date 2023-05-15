using System.Diagnostics.CodeAnalysis;
using FlexFramework.Core.Rendering.BackgroundRenderers;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.PostProcessing;

namespace FlexFramework.Core.Rendering;

public class CommandList
{
    private Dictionary<LayerType, List<IDrawData>> drawDatas = new();
    private List<PostProcessor> postProcessors = new();
    private BackgroundRenderer? backgroundRenderer;
    private CameraData backgroundCameraData;

    public bool TryGetLayer(LayerType layerType, [NotNullWhen(true)] out IReadOnlyList<IDrawData>? layer)
    {
        if (drawDatas.TryGetValue(layerType, out var drawData) && drawData.Count > 0)
        {
            layer = drawData;
            return true;
        }

        layer = null;
        return false;
    }

    public bool TryGetPostProcessors([NotNullWhen(true)] out IReadOnlyList<PostProcessor>? postProcessors)
    {
        postProcessors = this.postProcessors.Count > 0 ? this.postProcessors : null;
        return this.postProcessors.Count > 0;
    }

    public bool TryGetBackgroundRenderer([NotNullWhen(true)] out BackgroundRenderer? backgroundRenderer, out CameraData cameraData)
    {
        if (this.backgroundRenderer != null)
        {
            backgroundRenderer = this.backgroundRenderer;
            cameraData = backgroundCameraData;
            return true;
        }

        backgroundRenderer = null;
        cameraData = default;
        return false;
    }

    public void AddDrawData(LayerType layerType, IDrawData drawData)
    {
        var layer = GetLayerInternal(layerType);
        layer.Add(drawData);
    }

    public void AddPostProcessor(PostProcessor postProcessor)
    {
        postProcessors.Add(postProcessor);
    }

    public void UseBackgroundRenderer(BackgroundRenderer backgroundRenderer, CameraData cameraData)
    {
        this.backgroundRenderer = backgroundRenderer;
        backgroundCameraData = cameraData;
    }

    private List<IDrawData> GetLayerInternal(LayerType layerType)
    {
        if (drawDatas.TryGetValue(layerType, out var layer))
        {
            return layer;
        }

        var newLayer = new List<IDrawData>();
        drawDatas.Add(layerType, newLayer);
        return newLayer;
    }

    public void Clear()
    {
        foreach (var layer in drawDatas.Values)
        {
            layer.Clear();
        }
        
        postProcessors.Clear();
        backgroundRenderer = null;
    }
}