using OpenTK.Mathematics;

namespace FlexFramework.Util;

public static class Extensions
{
    public static Matrix4 ToMatrix4(this Matrix4d matrix4d)
    {
        return new Matrix4(
            (float) matrix4d.M11, (float) matrix4d.M12, (float) matrix4d.M13, (float) matrix4d.M14,
            (float) matrix4d.M21, (float) matrix4d.M22, (float) matrix4d.M23, (float) matrix4d.M24,
            (float) matrix4d.M31, (float) matrix4d.M32, (float) matrix4d.M33, (float) matrix4d.M34,
            (float) matrix4d.M41, (float) matrix4d.M42, (float) matrix4d.M43, (float) matrix4d.M44);
    }
}