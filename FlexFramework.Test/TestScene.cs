using System.Drawing;
using System.Globalization;
using FlexFramework.Audio;
using FlexFramework.Core;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
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
    private int transparentLayerId;
    private int guiLayerId;

    private Font robotoRegular;

    private AudioSource source;
    
    private MeshEntity[] meshEntities;
    private Vector3d[] scales;
    private Vector3d[] positions;
    
    private FilterButterworth[] filtersLp;
    private FilterButterworth[] filtersHp;

    private double scale = 0.0f;
    
    private Vector2d mouseVector = Vector2d.Zero;

    private int lastSample = 0;
    
    public override void Init()
    {
        opaqueLayerId = Engine.Renderer.GetLayerId(DefaultRenderer.OpaqueLayerName);
        transparentLayerId = Engine.Renderer.GetLayerId(DefaultRenderer.TransparentLayerName);
        guiLayerId = Engine.Renderer.GetLayerId(DefaultRenderer.GuiLayerName);

        source = new AudioSource();
        source.Clip = AudioClip.FromWave("HymnRisenRemix.wav");
        source.Play();
        
        camera = new OrthographicCamera();
        camera.Size = 5.0;
        camera.DepthNear = -10.0;
        camera.DepthFar = 10.0;

        robotoRegular = Engine.TextResources.GetFont("roboto-regular");

        fpsEntity = new TextEntity();

        const int barCount = 64;
        filtersLp = new FilterButterworth[barCount];
        filtersHp = new FilterButterworth[barCount];
        meshEntities = new MeshEntity[barCount];
        scales = new Vector3d[barCount];
        positions = new Vector3d[barCount];
        
        float noteStep = 120.0f / barCount;
        float a = MathF.Pow(2, 1.0f / 12.0f);
        float l = 8.0f;
        float h = 16.0f;
        
        for (int i = 0; i < barCount; i++)
        {
            filtersLp[i] = new FilterButterworth(h, source.Clip.SampleRate,
                FilterButterworth.PassType.Lowpass, MathF.Sqrt(2.0f));
            filtersHp[i] = new FilterButterworth(l, source.Clip.SampleRate,
                FilterButterworth.PassType.Highpass, MathF.Sqrt(2.0f));
            
            l = h;
            h = l * MathF.Pow(a, noteStep);
            
            meshEntities[i] = new MeshEntity();
            meshEntities[i].Mesh = Engine.PersistentResources.QuadMesh;
            positions[i] = new Vector3d(MathHelper.Lerp(-4.0, 4.0, i / (double) (barCount - 1)), 0.0, 0.0);
        }
    }

    public override void Update(UpdateArgs args)
    {
        TextBuilder textBuilder = new TextBuilder(Engine.TextResources.Fonts)
            .WithBaselineOffset(-robotoRegular.Height)
            .AddText(new StyledText(Math.Floor(1.0 / args.DeltaTime).ToString(CultureInfo.InvariantCulture), robotoRegular)
                .WithColor(Color.White));
        fpsEntity.SetText(textBuilder.Build());

        Vector2 mousePos = Engine.MouseState.Position;
        mousePos = (Vector2) Engine.ClientSize * 0.5f - mousePos;
        
        mouseVector = mousePos / ((Vector2) Engine.ClientSize * 0.5f);

        int pos = source.SamplePosition;
        for (int i = lastSample; i < pos; i++)
        {
            for (int j = 0; j < meshEntities.Length; j++)
            {
                filtersLp[j].Update(source.Clip.Samples[0, i]);
                filtersHp[j].Update(filtersLp[j].Value);
            }
        }
        lastSample = pos;
        for (int i = 0; i < meshEntities.Length; i++)
        {
            float filterValue = MathF.Abs(filtersHp[i].Value);

            scales[i] = new Vector3d(0.075, MathHelper.Lerp(scales[i].Y,  filterValue * 32.0 + 0.1, args.DeltaTime * 10.0), 1.0);
        }
        scale = MathHelper.Lerp(scale, Math.Abs(filtersHp[11].Value * 2.0), args.DeltaTime * 4.0);
    }

    public override void Render(Renderer renderer)
    {
        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);

        transform.Push();
        transform.Translate(-Engine.ClientSize.X / 2.0, Engine.ClientSize.Y / 2.0, 0.0);
        transform.Scale(camera.Size / Engine.ClientSize.Y, camera.Size / Engine.ClientSize.Y, 1.0);
        fpsEntity.Render(renderer, guiLayerId, transform, cameraData);
        transform.Pop();
        
        transform.Push();
        transform.Translate(mouseVector.X * 0.05, -mouseVector.Y * 0.05, 0.0);

        transform.Push();
        transform.Scale(scale + 1.0, scale + 1.0, 0.0);
        for (int i = 0; i < meshEntities.Length; i++)
        {
            transform.Push();
            transform.Scale(scales[i]);
            transform.Translate(positions[i]);
            meshEntities[i].Render(renderer, opaqueLayerId, transform, cameraData);
            transform.Pop();
        }
        transform.Pop();
        
        transform.Push();
        transform.Scale(1.0 + scale * 0.5, 1.0 - scale * 0.5, 1.0);
        transform.Translate(1.5, -1.5 + scale, 0.2);
        
        transform.Pop();
        transform.Pop();
    }

    public override void Dispose()
    {
        fpsEntity.Dispose();
    }
}