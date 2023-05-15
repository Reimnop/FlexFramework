using FlexFramework;
using FlexFramework.Core.Rendering.Renderers;
using FlexFramework.Test;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

NativeWindowSettings nws = new NativeWindowSettings()
{
    Title = "FlexFramework Test",
    Size = new Vector2i(1366, 768),
    APIVersion = new Version(3, 2),
    Profile = ContextProfile.Core
};

using (FlexFrameworkMain flexFramework = new FlexFrameworkMain(nws))
{
    flexFramework.UseRenderer(new DefaultRenderer());
    var textAssetsLocation = flexFramework.DefaultAssets.TextAssets;
    var textAssets = flexFramework.ResourceRegistry.GetResource(textAssetsLocation);
    
    textAssets.LoadFont("Fonts/Roboto-Regular.ttf", "roboto-regular", 24);
    flexFramework.LoadScene(new TestScene(flexFramework));

    while (!flexFramework.ShouldClose())
    {
        flexFramework.Update();
    }
}