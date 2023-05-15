using FlexFramework.Core;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public struct Transform
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Scale { get; set; } = Vector2.One;
    public float Rotation { get; set; } = 0.0f;

    public Transform(Vector2 position, Vector2 scale, float rotation)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }
    
    internal void ApplyToMatrixStack(MatrixStack matrixStack)
    {
        matrixStack.Scale(Scale.X, Scale.Y, 1.0f);
        matrixStack.Rotate(Vector3.UnitZ, Rotation);
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
    }
}