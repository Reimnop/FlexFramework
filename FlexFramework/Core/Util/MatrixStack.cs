using OpenTK.Mathematics;

namespace FlexFramework.Core.Util;

public class MatrixStack
{
    private struct LocalGlobalPair
    {
        public Matrix4d LocalTransformation;
        public Matrix4d GlobalTransformation;

        public LocalGlobalPair(Matrix4d local, Matrix4d global)
        {
            LocalTransformation = local;
            GlobalTransformation = global;
        }
    }

    private readonly Stack<LocalGlobalPair> internalStack = new Stack<LocalGlobalPair>();
    
    public Matrix4d LocalTransformation { get; private set; } = Matrix4d.Identity;
    public Matrix4d GlobalTransformation { get; private set; } = Matrix4d.Identity;

    public void Push()
    {
        internalStack.Push(new LocalGlobalPair(LocalTransformation, GlobalTransformation));
        LocalTransformation = Matrix4d.Identity;
    }

    public void PushMatrix(Matrix4d matrix)
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

    public void Transform(Matrix4d matrix)
    {
        LocalTransformation *= matrix;
        GlobalTransformation = LocalTransformation * internalStack.Peek().GlobalTransformation;
    }

    public void Translate(Vector3d value)
    {
        Transform(Matrix4d.CreateTranslation(value));
    }
    
    public void Translate(double x, double y, double z)
    {
        Transform(Matrix4d.CreateTranslation(x, y, z));
    }
    
    public void Scale(Vector3d value)
    {
        Transform(Matrix4d.Scale(value));
    }
    
    public void Scale(double x, double y, double z)
    {
        Transform(Matrix4d.Scale(x, y, z));
    }
    
    public void Rotate(Quaterniond value)
    {
        Transform(Matrix4d.CreateFromQuaternion(value));
    }
    
    public void Rotate(Vector3d axis, double angle)
    {
        Transform(Matrix4d.CreateFromAxisAngle(axis, angle));
    }

    public void Reset()
    {
        internalStack.Clear();
        LocalTransformation = Matrix4d.Identity;
        GlobalTransformation = Matrix4d.Identity;
    }
}