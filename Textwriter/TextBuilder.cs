using System.Numerics;
using System.Text.RegularExpressions;

namespace Textwriter;

public class TextBuilder
{
    private class Line
    {
        public List<BuiltGlyph> Glyphs { get; } = new List<BuiltGlyph>();
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Line(int height)
        {
            Height = height;
        }
        
        public void AddGlyph(BuiltGlyph glyph, int width, int height)
        {
            Glyphs.Add(glyph);
            Width += width;
            Height = Math.Max(Height, height);
        }
    }
    
    private readonly List<StyledText> styledTexts = new List<StyledText>();
    private readonly Dictionary<AtlasTexture<Vector3>, int> atlases = new Dictionary<AtlasTexture<Vector3>, int>();
    private int minLineHeight = 0;
    private int baselineOffset = 0;
    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
    
    /// <param name="minLineHeight">Use height of first font if null</param>
    public TextBuilder(int? minLineHeight, IReadOnlyList<Font> fonts)
    {
        for (int i = 0; i < fonts.Count; i++)
        {
            atlases.Add(fonts[i].Atlas, i);
        }
        this.minLineHeight = minLineHeight ?? fonts[0].Height;
    }

    public TextBuilder AddText(StyledText text)
    {
        styledTexts.Add(text);
        return this;
    }

    public TextBuilder AddTexts(ICollection<StyledText> texts)
    {
        styledTexts.AddRange(texts);
        return this;
    }

    public TextBuilder WithBaselineOffset(int value)
    {
        baselineOffset = value;
        return this;
    }
    
    public TextBuilder WithHorizontalAlignment(HorizontalAlignment value)
    {
        horizontalAlignment = value;
        return this;
    }
    
    public TextBuilder WithVerticalAlignment(VerticalAlignment value)
    {
        verticalAlignment = value;
        return this;
    }

    public BuiltText Build()
    {
        List<Line> builtLines = new List<Line>();
        foreach (List<StyledText> line in EnumerateTextLines(styledTexts))
        {
            Line builtLine = new Line(minLineHeight);
            int advance = 0;
            foreach (var (glyphInfo, style) in EnumerateStyledTexts(line))
            {
                BuiltGlyph glyph = new BuiltGlyph(
                    style, glyphInfo.Font, 
                    advance, 0, 
                    glyphInfo.Index, atlases[glyphInfo.Font.Atlas]);
                builtLine.AddGlyph(glyph, glyphInfo.AdvanceX, glyphInfo.Font.Height);
                advance += glyphInfo.AdvanceX;
            }
            builtLines.Add(builtLine);
        }

        int textOffsetY = 0;
        foreach (Line line in builtLines)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    textOffsetY -= line.Height;
                    break;
                case VerticalAlignment.Center:
                    textOffsetY -= line.Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    break;
            }
        }
        
        List<BuiltGlyph> glyphs = new List<BuiltGlyph>();
        int advanceY = textOffsetY + baselineOffset;
        foreach (Line line in builtLines)
        {
            int offsetX = 0;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    break;
                case HorizontalAlignment.Center:
                    offsetX = -line.Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    offsetX = -line.Width;
                    break;
            }
            
            foreach (BuiltGlyph glyph in line.Glyphs)
            {
                BuiltGlyph currentGlyph = glyph;
                currentGlyph.OffsetX += offsetX;
                currentGlyph.OffsetY = advanceY;
                glyphs.Add(currentGlyph);
            }
            advanceY += line.Height;
        }

        return new BuiltText(glyphs);
    }

    private static IEnumerable<List<StyledText>> EnumerateTextLines(IEnumerable<StyledText> texts)
    {
        Regex regex = new Regex(@"\n|\r\n");
        List<StyledText> currentLine = new List<StyledText>();
        foreach (StyledText text in texts)
        {
            string[] lines = regex.Split(text.Text);
            if (lines.Length > 1)
            {
                currentLine.Add(new StyledText(lines[0], text.Style, text.Font));
                yield return currentLine;
                
                for (int i = 1; i < lines.Length; i++)
                {
                    yield return new List<StyledText>()
                    {
                        new StyledText(lines[i], text.Style, text.Font)
                    };
                }
                
                currentLine.Clear();
            }
            else
            {
                currentLine.Add(text);
            }
        }
        
        if (currentLine.Count > 0)
        {
            yield return currentLine;
        }
    }

    private static IEnumerable<(GlyphInfo, Style)> EnumerateStyledTexts(IEnumerable<StyledText> texts)
    {
        foreach (StyledText text in texts)
        {
            // Shape the text
            foreach (GlyphInfo glyphInfo in text.Font.ShapeText(text.Text))
            {
                yield return (glyphInfo, text.Style);
            }
        }
    }
}