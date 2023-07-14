using FlexFramework.Modelling.Animation;
using FlexFramework.Util;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace FlexFramework.Modelling;

public class AnimationHandler
{
    private class TransformableNode
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        private readonly ModelNode node;
        
        public TransformableNode(ModelNode node)
        {
            this.node = node;
            ResetTransformation();
        }

        public void ResetTransformation()
        {
            var transform = node.Transform;
            Position = transform.ExtractTranslation();
            Scale = transform.ExtractScale();
            Rotation = transform.ExtractRotation();
        }
    }
    
    private Node<ModelNode> rootModelNode;
    private Dictionary<string, TransformableNode> nodes;

    private ModelAnimation? animation;

    public AnimationHandler(Model model)
    {
        rootModelNode = model.RootNode;
        nodes = model.RootNode.ToDictionary(node => node.Value.Name, node => new TransformableNode(node.Value));
    }
    
    public void Transition(ModelAnimation? animation)
    {
        this.animation = animation;
        
        if (this.animation == null)
            foreach (var node in nodes.Values)
                node.ResetTransformation();
    }

    public void Update(float time)
    {
        if (animation == null) 
            return;

        var t = (time * animation.TicksPerSecond) % animation.DurationInTicks;

        foreach (var (name, node) in nodes)
        {
            var channel = animation.GetAnimationChannelFromName(name);
            if (channel == null) 
                continue;
            
            node.Position = Sequence.Interpolate(t, channel.PositionKeys, Vector3Lerp);
            node.Scale = Sequence.Interpolate(t, channel.ScaleKeys, Vector3Lerp);
            node.Rotation = Sequence.Interpolate(t, channel.RotationKeys, Quaternion.Slerp);
        }
    }

    private static Vector3 Vector3Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(
                MathHelper.Lerp(a.X, b.X, t), 
                MathHelper.Lerp(a.Y, b.Y, t), 
                MathHelper.Lerp(a.Z, b.Z, t)
            );
    }

    public Matrix4 GetNodeTransform(ModelNode node)
    {
        var transformableNode = nodes[node.Name];
        return Matrix4.CreateScale(transformableNode.Scale) * 
               Matrix4.CreateFromQuaternion(transformableNode.Rotation) *
               Matrix4.CreateTranslation(transformableNode.Position);
    }
}