using System.Diagnostics;

namespace FlexFramework.Text;

/// <summary>
/// Utility class for shaping text.
/// </summary>
public static class TextShaper
{
    [Conditional("DEBUG")] // Potentially expensive, so only run in debug builds.
    private static void EnsureNoLineBreaks(string line)
    {
        if (line.Contains('\n'))
        {
            throw new ArgumentException("Line must not contain line breaks.", nameof(line));
        }
    }

    private static ShapedGlyph GetShapedGlyph(GlyphInfo glyph, float emSize, int x, int y)
    {
        var minPosX = (int) (glyph.Metrics.HorizontalBearingX * emSize);
        var minPosY = (int) (-glyph.Metrics.HorizontalBearingY * emSize);
        var maxPosX = (int) (minPosX + glyph.Metrics.Width * emSize);
        var maxPosY = (int) (minPosY + glyph.Metrics.Height * emSize);
        var minTexX = glyph.TextureCoordinates.MinX;
        var minTexY = glyph.TextureCoordinates.MinY;
        var maxTexX = glyph.TextureCoordinates.MaxX;
        var maxTexY = glyph.TextureCoordinates.MaxY;
        minPosX += x;
        minPosY += y;
        maxPosX += x;
        maxPosY += y;
        
        return new ShapedGlyph(
            minPosX, minPosY, maxPosX, maxPosY,
            minTexX, minTexY, maxTexX, maxTexY);
    }

    public static ShapedText ShapeText(
        Font font, 
        string text,
        int boundsMinX,
        int boundsMinY,
        int boundsMaxX,
        int boundsMaxY,
        float emSize = 1.0f,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, 
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var offsetY = GetTextOffsetY(font, text, boundsMinY, boundsMaxY, emSize, verticalAlignment);
        
        var lines = new List<GlyphLine>();
        foreach (var line in text.Split('\n'))
        {
            var offsetX = GetLineOffsetX(font, line, boundsMinX, boundsMaxX, emSize, horizontalAlignment);
            lines.Add(ShapeLine(font, line, emSize, offsetX, offsetY));
            offsetY += font.Metrics.Height;
        }
        
        return new ShapedText(font, lines);
    }
    
    public static TextBounds GetTextBounds(
        Font font,
        string text,
        int boundsMinX,
        int boundsMinY,
        int boundsMaxX,
        int boundsMaxY,
        float emSize = 1.0f,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var offsetY = GetTextOffsetY(font, text, boundsMinY, boundsMaxY, emSize, verticalAlignment);
        
        var index = -1;
        var lines = new List<LineBounds>();
        foreach (var line in text.Split('\n'))
        {
            var offsetX = GetLineOffsetX(font, line, boundsMinX, boundsMaxX, emSize, horizontalAlignment);
            lines.Add(GetLineBounds(font, line, emSize, offsetX, offsetY, ref index));
            offsetY += font.Metrics.Height;
        }
        
        return new TextBounds(lines);
    }

    public static LineBounds GetLineBounds(Font font, string line, float emSize, int offsetX, int offsetY, ref int index)
    {
        var top = offsetY - font.Metrics.Ascent;
        var bottom = offsetY - font.Metrics.Descent;
        if (line.Length == 0)
            return new LineBounds(
                top, 
                bottom, 
                Enumerable.Empty<int>().Append(offsetX), 
                Enumerable.Empty<int>().Append(index++));

        var selectablePositions = new List<int> {offsetX};
        var selectableIndices = new List<int> {index};
        index++;
        var currentX = offsetX;
        for (int i = 0; i < line.Length - 1; i++)
        {
            var left = line[i];
            var right = line[i + 1];
            var glyph = font.GetGlyph(left);
            currentX += (int) ((glyph.Metrics.AdvanceX + font.GetKerning(left, right)) * emSize);
            selectablePositions.Add(currentX);
            selectableIndices.Add(index);
            index++;
        }

        selectablePositions.Add(currentX + (int) (font.GetGlyph(line[^1]).Metrics.AdvanceX * emSize));
        selectableIndices.Add(index);
        index++;
        return new LineBounds(top, bottom, selectablePositions, selectableIndices);
    }

    public static GlyphLine ShapeLine(Font font, string line, float emSize, int x, int y)
    {
        EnsureNoLineBreaks(line);
        
        if (line.Length == 0)
            return new GlyphLine(Enumerable.Empty<ShapedGlyph>()); // Empty line has no glyphs.

        var glyphs = new List<ShapedGlyph>(line.Length);
        var offsetX = x;
        var offsetY = y;
        for (int i = 0; i < line.Length - 1; i++)
        {
            var left = line[i];
            var right = line[i + 1];
            var glyph = font.GetGlyph(left);

            if (glyph.Metrics.Width * glyph.Metrics.Height != 0) // Skip empty glyphs (e.g. spaces).
                glyphs.Add(GetShapedGlyph(glyph, emSize, offsetX, offsetY));

            offsetX += (int) ((glyph.Metrics.AdvanceX + font.GetKerning(left, right)) * emSize);
        }

        var lastGlyph = font.GetGlyph(line[^1]);
        if (lastGlyph.Metrics.Width * lastGlyph.Metrics.Height != 0) // Skip empty glyphs (e.g. spaces).
            glyphs.Add(GetShapedGlyph(lastGlyph, emSize, offsetX, offsetY));

        return new GlyphLine(glyphs);
    }

    public static int GetLineOffsetX(Font font, string line, int minX, int maxX, float emSize, HorizontalAlignment horizontalAlignment)
    {
        EnsureNoLineBreaks(line);

        return horizontalAlignment switch
        {
            HorizontalAlignment.Left => minX,
            HorizontalAlignment.Center => -(CalculateLineWidth(font, line, emSize) >> 1) + ((maxX - minX) >> 1) + minX, // Divide by 2
            HorizontalAlignment.Right => -CalculateLineWidth(font, line, emSize) + maxX,
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
        };
    }
    
    public static int GetTextOffsetY(Font font, string line, int minY, int maxY, float emSize, VerticalAlignment verticalAlignment)
    {
        return verticalAlignment switch
        {
            VerticalAlignment.Bottom => -CalculateTextHeight(font, line, emSize) + (int) ((font.Metrics.Height + font.Metrics.Descent) * emSize) + maxY,
            VerticalAlignment.Center => -(CalculateTextHeight(font, line, emSize) >> 1) + ((maxY - minY) >> 1) + (int) (font.Metrics.Height * emSize) + (int) (font.Metrics.Descent * emSize) + minY, // Divide by 2
            VerticalAlignment.Top => (int) (font.Metrics.Ascent * emSize) + minY,
            _ => throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, null)
        };
    }

    public static int CalculateTextHeight(Font font, string text, float emSize)
    {
        var lineBreaks = text.Count(c => c == '\n');
        return (int) (font.Metrics.Height * (lineBreaks + 1) * emSize);
    }
    
    public static int CalculateLineWidth(Font font, string line, float emSize)
    {
        EnsureNoLineBreaks(line);

        if (line.Length == 0)
            return 0; // Empty line has no width.
        
        if (line.Length == 1)
            return (int) (font.GetGlyph(line[0]).Metrics.AdvanceX * emSize); // Single character line has the width of that character.
        
        int width = 0;
        for (int i = 0; i < line.Length - 1; i++)
        {
            char left = line[i];
            char right = line[i + 1];
            width += (int) ((font.GetGlyph(left).Metrics.AdvanceX + font.GetKerning(left, right)) * emSize);
        }
        
        width += (int) (font.GetGlyph(line[^1]).Metrics.AdvanceX * emSize); // Add the width of the last character.
        return width;
    }
}