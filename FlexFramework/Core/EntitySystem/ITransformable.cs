using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem;

public interface ITransformable
{
    Vector3d Position { get; set; }
    Vector3d Scale { get; set; }
    Quaterniond Rotation { get; set; }
}