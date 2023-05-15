using System.Drawing;

namespace Textwriter;

public static class TextMeshGenerator
{
     public static void GenerateVertices(BuiltText builtText, IList<TextVertex> outVertices)
     {
          outVertices.Clear();
          foreach (BuiltGlyph builtGlyph in builtText)
          {
               Style style = builtGlyph.Style;
               Color color = style.Color;
               Glyph glyph = builtGlyph.Font.GetGlyph(builtGlyph.Index);

               if (glyph.Width * glyph.Height == 0)
               {
                    continue;
               }

               float minX = (builtGlyph.OffsetX + glyph.HorizontalBearingX) / 64.0f;
               float minY = (builtGlyph.OffsetY - glyph.HorizontalBearingY) / 64.0f;
               float maxX = (builtGlyph.OffsetX + glyph.HorizontalBearingX + glyph.Width) / 64.0f;
               float maxY = (builtGlyph.OffsetY - glyph.HorizontalBearingY + glyph.Height) / 64.0f;
               float minU = glyph.Uv.Min.X;
               float minV = glyph.Uv.Max.Y;
               float maxU = glyph.Uv.Max.X;
               float maxV = glyph.Uv.Min.Y;

               outVertices.Add(new TextVertex(minX, minY, minU, maxV,
                    color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                    builtGlyph.TextureIndex));
               outVertices.Add(new TextVertex(maxX, maxY, maxU, minV,
                    color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                    builtGlyph.TextureIndex));
               outVertices.Add(new TextVertex(minX, maxY, minU, minV,
                    color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                    builtGlyph.TextureIndex));
               outVertices.Add(new TextVertex(minX, minY, minU, maxV,
                    color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                    builtGlyph.TextureIndex));
               outVertices.Add(new TextVertex(maxX, minY, maxU, maxV,
                    color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                    builtGlyph.TextureIndex));
               outVertices.Add(new TextVertex(maxX, maxY, maxU, minV,
                    color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                    builtGlyph.TextureIndex));
          }
     }
}