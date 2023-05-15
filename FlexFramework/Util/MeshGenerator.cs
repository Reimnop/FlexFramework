using OpenTK.Mathematics;

namespace FlexFramework.Util;

public static class MeshGenerator
{
    public static void GenerateRoundedRectangle(IList<Vector2> vertices, Vector2 min, Vector2 max, float radius, int resolution = 8)
    {
        if (radius == 0.0)
        {
            GenerateRectangle(vertices, min, max);
            return;
        }
        
        float maxRadius = Math.Min(max.X - min.X, max.Y - min.Y) * 0.5f;
        radius = Math.Min(radius, maxRadius);
        
        Vector2 a = new Vector2(max.X - radius, max.Y - radius);
        Vector2 b = new Vector2(min.X + radius, max.Y - radius);
        Vector2 c = new Vector2(min.X + radius, min.Y + radius);
        Vector2 d = new Vector2(max.X - radius, min.Y + radius);

        GenerateCircleArch(vertices, a, radius, resolution, 0.0f, MathF.PI * 0.5f);
        GenerateCircleArch(vertices, b, radius, resolution, MathF.PI * 0.5f, MathF.PI);
        GenerateCircleArch(vertices, c, radius, resolution, MathF.PI, MathF.PI * 1.5f);
        GenerateCircleArch(vertices, d, radius, resolution, MathF.PI * 1.5f, MathF.PI * 2.0f);
        GenerateRectangle(vertices, new Vector2(min.X, min.Y + radius), b);
        GenerateRectangle(vertices, d, new Vector2(max.X, max.Y - radius));
        GenerateRectangle(vertices, new Vector2(min.X + radius, min.Y), new Vector2(max.X - radius, max.Y));
    }

    public static void GenerateRectangle(IList<Vector2> vertices, Vector2 min, Vector2 max)
    {
        Vector2 lengths = max - min;
        if (lengths.X * lengths.Y == 0.0)
        {
            return;
        }
        
        vertices.Add(new Vector2(max.X, max.Y));
        vertices.Add(new Vector2(min.X, max.Y));
        vertices.Add(new Vector2(min.X, min.Y));
        vertices.Add(new Vector2(max.X, max.Y));
        vertices.Add(new Vector2(min.X, min.Y));
        vertices.Add(new Vector2(max.X, min.Y));
    }
    
    public static void GenerateCircleArch(IList<Vector2> vertices, Vector2 center, float radius, int resolution = 8, float startAngle = 0.0f, float endAngle = MathF.PI * 2.0f)
    {
        for (int i = 0; i < resolution; i++)
        {
            float alpha = MathHelper.Lerp(startAngle, endAngle, i / (float) resolution);
            float beta = MathHelper.Lerp(startAngle, endAngle, (i + 1) / (float) resolution);
            Vector2 a = new Vector2(MathF.Cos(alpha), MathF.Sin(alpha));
            Vector2 b = new Vector2(MathF.Cos(beta), MathF.Sin(beta));
            vertices.Add(center);
            vertices.Add(a * radius + center);
            vertices.Add(b * radius + center);
        }
    }
}