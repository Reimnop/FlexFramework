using FlexFramework;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Text;
using FlexFramework.Test;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

NativeWindowSettings nws = new NativeWindowSettings()
{
    Title = "FlexFramework Test",
    Size = new Vector2i(1366, 768),
    APIVersion = new Version(4, 3),
    Profile = ContextProfile.Core
};

using (FlexFrameworkMain flexFramework = new FlexFrameworkMain(nws))
{
    flexFramework.UseRenderer<DefaultRenderer>();
    flexFramework.LoadFonts(24, 2048,
        new FontFileInfo("roboto-regular", "Fonts/Roboto-Regular.ttf"));
    flexFramework.LoadScene<TestScene>();

    while (!flexFramework.ShouldClose())
    {
        flexFramework.Update();
    }
}