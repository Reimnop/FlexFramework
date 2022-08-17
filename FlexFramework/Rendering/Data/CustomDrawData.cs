using FlexFramework.Rendering.DefaultRenderingStrategies;

namespace FlexFramework.Rendering.Data;

public class CustomDrawData : IDrawData
{
    public DrawFunc DrawFunc { get; set; }

    public CustomDrawData(DrawFunc drawFunc)
    {
        DrawFunc = drawFunc;
    }
}