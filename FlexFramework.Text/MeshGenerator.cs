namespace FlexFramework.Text;

/// <summary>
/// Delegate for consuming vertices.
/// </summary>
public delegate void VertexConsumer(TextVertex vertex);

/// <summary>
/// Generates meshes from text.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Generates a mesh from the given text.
    /// </summary>
    /// <returns>The amount of vertices generated.</returns>
    public static int GenerateMesh(VertexConsumer vertexConsumer, ShapedText shapedText)
    {
        const float scale = 1.0f / 64.0f;
        
        int count = 0;
        
        foreach (var line in shapedText.Lines)
        {
            foreach (var shapedGlyph in line)
            {
                var minPosX = shapedGlyph.MinPositionX * scale;
                var minPosY = shapedGlyph.MinPositionY * scale;
                var maxPosX = shapedGlyph.MaxPositionX * scale;
                var maxPosY = shapedGlyph.MaxPositionY * scale;
                var minTexX = shapedGlyph.MinTextureCoordinateX;
                var minTexY = shapedGlyph.MinTextureCoordinateY;
                var maxTexX = shapedGlyph.MaxTextureCoordinateX;
                var maxTexY = shapedGlyph.MaxTextureCoordinateY;
                
                // Triangle 1
                vertexConsumer(new TextVertex(minPosX, minPosY, minTexX, maxTexY));
                vertexConsumer(new TextVertex(maxPosX, maxPosY, maxTexX, minTexY));
                vertexConsumer(new TextVertex(minPosX, maxPosY, minTexX, minTexY));
                
                // Triangle 2
                vertexConsumer(new TextVertex(minPosX, minPosY, minTexX, maxTexY));
                vertexConsumer(new TextVertex(maxPosX, minPosY, maxTexX, maxTexY));
                vertexConsumer(new TextVertex(maxPosX, maxPosY, maxTexX, minTexY));
                
                count += 6;
            }
        }
        
        return count;
    }
}