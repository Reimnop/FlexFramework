using System.Diagnostics;
using OpenTK.Mathematics;

namespace FlexFramework.Core;

public class MatrixStack
{
    private struct LocalGlobalPair
    {
        public Matrix4 LocalTransformation;
        public Matrix4 GlobalTransformation;

        public LocalGlobalPair(Matrix4 local, Matrix4 global)
        {
            LocalTransformation = local;
            GlobalTransformation = global;
        }
    }

    private readonly Stack<LocalGlobalPair> internalStack = new Stack<LocalGlobalPair>();
    
    public Matrix4 LocalTransformation { get; private set; } = Matrix4.Identity;
    public Matrix4 GlobalTransformation { get; private set; } = Matrix4.Identity;

    public void Push()
    {
        internalStack.Push(new LocalGlobalPair(LocalTransformation, GlobalTransformation));
        LocalTransformation = Matrix4.Identity;
    }

    public void PushMatrix(Matrix4 matrix)
    {
        Push();
        Transform(matrix);
    }

    public void Pop()
    {
        LocalGlobalPair pair = internalStack.Pop();
        LocalTransformation = pair.LocalTransformation;
        GlobalTransformation = pair.GlobalTransformation;
    }

    public void Transform(Matrix4 matrix)
    {
        LocalTransformation *= matrix;
        GlobalTransformation = LocalTransformation * internalStack.Peek().GlobalTransformation;
    }

    public void Translate(Vector3 value)
    {
        Transform(Matrix4.CreateTranslation(value));
    }
    
    public void Translate(float x, float y, float z)
    {
        Transform(Matrix4.CreateTranslation(x, y, z));
    }
    
    public void Scale(Vector3 value)
    {
        Transform(Matrix4.CreateScale(value));
    }
    
    public void Scale(float x, float y, float z)
    {
        Transform(Matrix4.CreateScale(x, y, z));
    }
    
    public void Rotate(Quaternion value)
    {
        Transform(Matrix4.CreateFromQuaternion(value));
    }
    
    public void Rotate(Vector3 axis, float angle)
    {
        Transform(Matrix4.CreateFromAxisAngle(axis, angle));
    }

    public void Clear()
    {
        internalStack.Clear();
        LocalTransformation = Matrix4.Identity;
        GlobalTransformation = Matrix4.Identity;
    }
    
    [Conditional("DEBUG")]
    public void AssertEmpty()
    {
        if (internalStack.Count != 0)
        {
            throw new InvalidOperationException("Matrix stack is not empty.");
        }
    }
}