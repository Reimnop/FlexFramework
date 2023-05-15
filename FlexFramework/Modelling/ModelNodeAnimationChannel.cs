using FlexFramework.Modelling.Animation;
using OpenTK.Mathematics;

namespace FlexFramework.Modelling;

public class ModelNodeAnimationChannel
{
    public string NodeName { get; }
    public IReadOnlyList<Key<Vector3>> PositionKeys => positionKeys;
    public IReadOnlyList<Key<Vector3>> ScaleKeys => scaleKeys;
    public IReadOnlyList<Key<Quaternion>> RotationKeys => rotationKeys;

    private readonly List<Key<Vector3>> positionKeys;
    private readonly List<Key<Vector3>> scaleKeys;
    private readonly List<Key<Quaternion>> rotationKeys;

    public ModelNodeAnimationChannel(string nodeName, IEnumerable<Key<Vector3>> positionKeys, IEnumerable<Key<Vector3>> scaleKeys, IEnumerable<Key<Quaternion>> rotationKeys)
    {
        NodeName = nodeName;
        this.positionKeys = positionKeys.ToList();
        this.scaleKeys = scaleKeys.ToList();
        this.rotationKeys = rotationKeys.ToList();
    }
}