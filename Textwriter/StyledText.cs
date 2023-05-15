using System.Drawing;

namespace Textwriter;

public class StyledText
{
    public string Text { get; set; }
    public Style Style { get; set; }
    public Font Font { get; set; }

    public StyledText(string text, Font font)
    {
        Text = text;
        Font = font;
    }

    public StyledText(string text, Style style, Font font) : this(text, font)
    {
        Style = style;
    }

    public StyledText WithStyle(Style style)
    {
        Style = style;
        return this;
    }

    public StyledText WithColor(Color value)
    {
        Style style = Style;
        style.Color = value;
        return WithStyle(style);
    }
}