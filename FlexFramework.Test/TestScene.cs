using System.Drawing;
using FlexFramework.Audio;
using FlexFramework.Core;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using Textwriter;
using Renderer = FlexFramework.Rendering.Renderer;

namespace FlexFramework.Test;

public class TestScene : Scene
{
    private OrthographicCamera camera;

    private MatrixStack transform = new MatrixStack();
    private TextEntity fpsEntity;

    private int opaqueLayerId;
    private int guiLayerId;

    private Font robotoRegular;

    private AudioSource source;

    public override void Init()
    {
        opaqueLayerId = Engine.Renderer.GetLayerId(DefaultRenderer.OpaqueLayerName);
        guiLayerId = Engine.Renderer.GetLayerId(DefaultRenderer.GuiLayerName);

        source = new AudioSource();
        source.Clip = AudioClip.FromWave("HymnRisenRemix.wav");
        source.Play();
        
        camera = new OrthographicCamera();
        camera.DepthNear = -10.0;
        camera.DepthFar = 10.0;

        robotoRegular = Engine.TextResources.GetFont("roboto-regular");

        fpsEntity = new TextEntity();
    }

    public override void Update(UpdateArgs args)
    {
        TextBuilder textBuilder = new TextBuilder(Engine.TextResources.Fonts)
            .WithBaselineOffset(-robotoRegular.Height)
            .AddText(new StyledText($"FPS: {Math.Floor(1.0 / args.DeltaTime)}", robotoRegular)
                .WithColor(Color.White));
        fpsEntity.SetText(textBuilder.Build());
    }

    public override void Render(Renderer renderer)
    {
        camera.Size = Engine.ClientSize.Y;
        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        
        transform.Push();
        transform.Translate(-Engine.ClientSize.X / 2.0, Engine.ClientSize.Y / 2.0, 0.0);
        fpsEntity.Render(renderer, guiLayerId, transform, cameraData);
        transform.Pop();
    }

    public override void Dispose()
    {
        fpsEntity.Dispose();
    }
}