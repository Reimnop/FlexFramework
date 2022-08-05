using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;

namespace FlexFramework.Test;

public class AnimatedEntity : Entity, IRenderable, ITransformable
{
    public Vector3d Position
    {
        get => texturedEntity.Position;
        set => texturedEntity.Position = value;
    }

    public Vector3d Scale
    {
        get => texturedEntity.Scale;
        set => texturedEntity.Scale = value;
    }
    
    public Quaterniond Rotation
    {
        get => texturedEntity.Rotation;
        set => texturedEntity.Rotation = value;
    }

    public Animation Animation { get; set; }

    private readonly TexturedEntity texturedEntity;

    public AnimatedEntity(FlexFrameworkMain engine)
    {
        texturedEntity = new TexturedEntity(engine);
    }
        
    public override void Update(UpdateArgs args)
    {
        int frame = (int) (args.Time * Animation.Framerate) % Animation.AnimationFrames.Count;
        texturedEntity.Texture = Animation.AnimationFrames[frame];
        
        texturedEntity.Update(args);
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        texturedEntity.Render(renderer, layerId, matrixStack, cameraData);
    }

    public override void Dispose()
    {
        texturedEntity.Dispose();
    }
}