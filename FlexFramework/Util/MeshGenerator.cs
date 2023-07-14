using OpenTK.Mathematics;
using Poly2Tri;
using Poly2Tri.Triangulation.Polygon;
using Poly2Tri.Utility;

namespace FlexFramework.Util;

public delegate void VertexConsumer(Vector2 vertex);

public static class MeshGenerator
{
    public static int GenerateRoundedRectangle(VertexConsumer vertexConsumer, Box2 bounds, float radius, float borderThickness = float.PositiveInfinity, int resolution = 8)
    {
        var polygon = GenerateRectanglePoly(bounds, radius, resolution);
        
        // Generate inner path if border thickness is less than half of the smallest dimension
        var maxThickness = MathF.Min(bounds.HalfSize.X, bounds.HalfSize.Y);
        if (borderThickness < maxThickness)
        {
            var innerBounds = new Box2(bounds.Min + new Vector2(borderThickness), bounds.Max - new Vector2(borderThickness));
            var innerRadius = MathF.Max(radius - borderThickness, 0.0f);
            polygon.AddHole(GenerateRectanglePoly(innerBounds, innerRadius, resolution));
        }
        
        // Triangulate
        P2T.Triangulate(polygon);
        
        // Generate vertices
        foreach (var triangle in polygon.Triangles)
        {
            vertexConsumer(Point2DToVector2(triangle.Points[0]));
            vertexConsumer(Point2DToVector2(triangle.Points[1]));
            vertexConsumer(Point2DToVector2(triangle.Points[2]));
        }
        
        return polygon.Triangles.Count * 3;
    }
    
    private static Vector2 Point2DToVector2(Point2D point)
    {
        return new Vector2((float) point.X, (float) point.Y);
    }

    private static Polygon GenerateRectanglePoly(Box2 bounds, float radius, int resolution)
    {
        var min = bounds.Min;
        var max = bounds.Max;

        // This is what LINQ does to you, kids
        var points = EnumerateCircleArch(new Vector2(max.X - radius, max.Y - radius), radius, resolution,
                MathF.PI * 0.0f, MathF.PI * 0.5f)
            .Append(new PolygonPoint(max.X - radius, max.Y))
            .Concat(EnumerateCircleArch(new Vector2(min.X + radius, max.Y - radius), radius, resolution,
                MathF.PI * 0.5f, MathF.PI * 1.0f))
            .Append(new PolygonPoint(min.X, max.Y - radius))
            .Concat(EnumerateCircleArch(new Vector2(min.X + radius, min.Y + radius), radius, resolution,
                MathF.PI * 1.0f, MathF.PI * 1.5f))
            .Append(new PolygonPoint(min.X + radius, min.Y))
            .Concat(EnumerateCircleArch(new Vector2(max.X - radius, min.Y + radius), radius, resolution,
                MathF.PI * 1.5f, MathF.PI * 2.0f))
            .Append(new PolygonPoint(max.X, min.Y + radius));
        
        return new Polygon(points.ToArray());
    }

    private static IEnumerable<PolygonPoint> EnumerateCircleArch(Vector2 center, float radius, int resolution, float startAngle, float endAngle)
    {
        if (radius == 0.0f)
            yield break;
        
        for (int i = 0; i < resolution; i++)
        {
            var theta = MathHelper.Lerp(startAngle, endAngle, i / (float) resolution);
            yield return new PolygonPoint(MathF.Cos(theta) * radius + center.X, MathF.Sin(theta) * radius + center.Y);
        }
    }
}