using System.Globalization;
using System.Runtime.InteropServices;
using FlexFramework.Core.Audio;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;

namespace FlexFramework.Test;

public class TestScene : Scene
{
    private OrthographicCamera camera;

    private MatrixStack transform = new MatrixStack();
    
    private TextEntity fpsEntity;

    private float[,] samples;
    private AudioStream stream;
    private AudioSource source;
    
    private MeshEntity[] meshEntities;
    private Vector3[] scales;
    private Vector3[] positions;
    
    private FilterButterworth[] filtersLp;
    private FilterButterworth[] filtersHp;

    private float scale = 0.0f;
    
    private Vector2 mouseVector = Vector2.Zero;

    private long lastSample = 0L;

    private CommandList commandList = new();
    private IRenderBuffer renderBuffer;
    
    public TestScene(FlexFrameworkMain engine) : base(engine)
    {
        renderBuffer = Engine.Renderer.CreateRenderBuffer(Engine.ClientSize);
        
        stream = new VorbisAudioStream("HymnRisenRemix.ogg");
        // get all the samples in advance
        samples = new float[2, stream.SampleCount];
        int pos = 0;
        while (stream.NextBuffer(out var data))
        {
            // copy data to samples using memory marshal
            var span = MemoryMarshal.Cast<byte, float>(data);
            for (int i = 0; i < span.Length; i += 2)
            {
                samples[0, i / 2 + pos] = span[i + 0];
                samples[1, i / 2 + pos] = span[i + 1];
            }
            pos += span.Length / 2;
        }
        stream.Seek(0L);
        
        source = new AudioSource();
        source.AudioStream = stream;
        source.Play();
        
        camera = new OrthographicCamera();
        camera.Size = 5.0f;
        camera.DepthNear = -10.0f;
        camera.DepthFar = 10.0f;
        
        var robotoRegular = Engine.ResourceRegistry.GetResource(Engine.DefaultAssets.TextAssets)["roboto-regular"];
        fpsEntity = new TextEntity(Engine, robotoRegular);

        const int barCount = 64;
        filtersLp = new FilterButterworth[barCount];
        filtersHp = new FilterButterworth[barCount];
        meshEntities = new MeshEntity[barCount];
        scales = new Vector3[barCount];
        positions = new Vector3[barCount];
        
        float noteStep = 120.0f / barCount;
        float a = MathF.Pow(2, 1.0f / 12.0f);
        float l = 8.0f;
        float h = 16.0f;
        
        for (int i = 0; i < barCount; i++)
        {
            filtersLp[i] = new FilterButterworth(h, stream.SampleRate,
                FilterButterworth.PassType.Lowpass, MathF.Sqrt(2.0f));
            filtersHp[i] = new FilterButterworth(l, stream.SampleRate,
                FilterButterworth.PassType.Highpass, MathF.Sqrt(2.0f));
            
            l = h;
            h = l * MathF.Pow(a, noteStep);
            
            meshEntities[i] = new MeshEntity();
            meshEntities[i].Mesh = Engine.ResourceRegistry.GetResource(Engine.DefaultAssets.QuadMesh);
            positions[i] = new Vector3(MathHelper.Lerp(-4.0f, 4.0f, i / (float) (barCount - 1)), 0.0f, 0.0f);
        }
    }

    public override void Update(UpdateArgs args)
    {
        fpsEntity.Text = Math.Floor(1.0 / args.DeltaTime).ToString(CultureInfo.InvariantCulture);

        Vector2 mousePos = Engine.Input.MousePosition;
        mousePos = (Vector2) Engine.ClientSize * 0.5f - mousePos;
        
        mouseVector = mousePos / ((Vector2) Engine.ClientSize * 0.5f);

        long pos = stream.SamplePosition;
        for (long i = lastSample; i < pos; i++)
        {
            for (int j = 0; j < meshEntities.Length; j++)
            {
                filtersLp[j].Update(samples[0, i]);
                filtersHp[j].Update(filtersLp[j].Value);
            }
        }
        lastSample = pos;
        for (int i = 0; i < meshEntities.Length; i++)
        {
            float filterValue = MathF.Abs(filtersHp[i].Value);

            scales[i] = new Vector3(0.075f, MathHelper.Lerp(scales[i].Y,  filterValue * 32.0f + 0.1f, args.DeltaTime * 10.0f), 1.0f);
        }
        scale = MathHelper.Lerp(scale, MathF.Abs(filtersHp[11].Value * 2.0f), args.DeltaTime * 4.0f);
    }

    public override void Render(Renderer renderer)
    {
        commandList.Clear();
        
        var cameraData = camera.GetCameraData(Engine.ClientSize);
        var guiArgs = new RenderArgs(commandList, LayerType.Gui, transform, cameraData);
        var opaqueArgs = new RenderArgs(commandList, LayerType.Opaque, transform, cameraData);

        transform.Push();
        transform.Scale(1.0f, -1.0f, 1.0f);
        transform.Translate(-Engine.ClientSize.X / 2.0f, Engine.ClientSize.Y / 2.0f, 0.0f);
        transform.Translate(0.0f, -24.0f, 0.0f);
        transform.Scale(camera.Size / Engine.ClientSize.Y, camera.Size / Engine.ClientSize.Y, 1.0f);
        fpsEntity.Render(guiArgs);
        transform.Pop();
        
        transform.Push();
        transform.Translate(mouseVector.X * 0.05f, -mouseVector.Y * 0.05f, 0.0f);

        transform.Push();
        transform.Scale(scale + 1.0f, scale + 1.0f, 0.0f);
        for (int i = 0; i < meshEntities.Length; i++)
        {
            transform.Push();
            transform.Scale(scales[i]);
            transform.Translate(positions[i]);
            meshEntities[i].Render(opaqueArgs);
            transform.Pop();
        }
        transform.Pop();
        
        transform.Push();
        transform.Scale(1.0f + scale * 0.5f, 1.0f - scale * 0.5f, 1.0f);
        transform.Translate(1.5f, -1.5f + scale, 0.2f);
        
        transform.Pop();
        transform.Pop();
        
        // Present screen
        renderer.Render(Engine.ClientSize, commandList, renderBuffer);
        Engine.Present(renderBuffer);
    }
}