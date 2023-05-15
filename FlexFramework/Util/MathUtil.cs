namespace FlexFramework.Util;

public static class MathUtil
{
    public static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}